using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ECommerceMVC.Models;

namespace ECommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private ECommerceDbContext db = new ECommerceDbContext();

        // GET: Cart
        public ActionResult Index()
        {
            var cartItems = db.Carts.Include(c => c.Product).Include(c => c.Customer).ToList();
            return View(cartItems);
        }

        // GET: Cart/Add/5
        public ActionResult Add(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            // For now, use customer ID = 1
            int customerId = 1;

            var existingCart = db.Carts.FirstOrDefault(c => c.ProductId == id && c.CustomerId == customerId);
            if (existingCart != null)
            {
                existingCart.ItemTotal += 1;
                existingCart.GrandTotal = existingCart.ItemTotal * product.MRP;
            }
            else
            {
                var cartItem = new Cart
                {
                    ProductId = product.ProductId,
                    CustomerId = customerId,
                    ItemTotal = 1,
                    GrandTotal = product.MRP
                };
                db.Carts.Add(cartItem);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Cart/Remove/5
        public ActionResult Remove(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cartItem = db.Carts.Find(id);
            if (cartItem == null) return HttpNotFound();

            db.Carts.Remove(cartItem);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Cart/Checkout
        public ActionResult Checkout()
        {
            int customerId = 1; // Replace with actual user ID in real app

            var cartItems = db.Carts
                              .Include(c => c.Product)
                              .Where(c => c.CustomerId == customerId)
                              .ToList();

            if (!cartItems.Any())
            {
                ViewBag.Message = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            // Create Order
            var order = new Order
            {
                CustomerId = customerId,
                CartId = cartItems.First().CartId, // Simplified
                OrderDate = DateTime.Now,
                ShippingDate = DateTime.Now.AddDays(3),
                OrderStatus = OrderStatus.Processing,
                OrderAmount = cartItems.Sum(i => i.GrandTotal)
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Create OrderItems
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    MRP = item.Product.MRP,
                    Quantity = item.ItemTotal
                };
                db.OrderItems.Add(orderItem);
            }

            db.SaveChanges();

            // Create Payment
            var payment = new Payment
            {
                OrderId = order.OrderId,
                CustomerId = customerId,
                PaymentMode = PaymentMode.Cash, // Set default or use ViewModel later
                DateOfPayment = DateTime.Now
            };
            db.Payments.Add(payment);

            // Clear Cart
            db.Carts.RemoveRange(cartItems);

            db.SaveChanges();

            TempData["Success"] = $"Order #{order.OrderId} placed successfully!";
            return RedirectToAction("Index");
            
        }
    }
}
