using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ECommerceMVC.Models; // Ensure PaymentMode and OrderStatus are defined here or in an imported namespace
using Microsoft.AspNet.Identity;
using System.Collections.Generic; // Added to support List<Cart>

namespace ECommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private ECommerceDbContext db = new ECommerceDbContext();

        // GET: Cart
        public ActionResult Index()
        {
            // Assume you have logic here to display the shopping cart
            // This part was modified to align with the logic used in the Checkout method
            string userId = User.Identity.GetUserId();
            var customer = db.Customers.FirstOrDefault(c => c.ApplicationUserId == userId);
            if (customer != null)
            {
                var cartItems = db.Carts.Include(c => c.Product)
                                    .Where(c => c.CustomerId == customer.CustomerId)
                                    .ToList();
                return View(cartItems);
            }
            // If there is no customer or cart, you can display an empty page or a message
            // ViewBag.Message = "Your cart is empty or you are not logged in."; // You can use ViewBag if you want
            return View(new List<Cart>());
        }

        // GET: Cart/Add/5
        public ActionResult Add(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            string userId = User.Identity.GetUserId();
            var customer = db.Customers.FirstOrDefault(c => c.ApplicationUserId == userId);
            if (customer == null)
            {
                // If the user is not logged in, or has no associated customer record, redirect to login page
                // while preserving the product they were trying to add, to return to it after login
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Add", "Cart", new { id = id }) });
            }

            int customerId = customer.CustomerId;

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
        public ActionResult Remove(int? id) // Assume id here is CartId or ProductId based on your design
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // If id is CartId
            var cartItem = db.Carts.Find(id);
            // If id is ProductId, you would also need CustomerId to uniquely identify the cart item
            // string userId = User.Identity.GetUserId();
            // var customer = db.Customers.FirstOrDefault(c => c.ApplicationUserId == userId);
            // if (customer == null) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            // var cartItem = db.Carts.FirstOrDefault(c => c.ProductId == id && c.CustomerId == customer.CustomerId);

            if (cartItem == null) return HttpNotFound();

            // (Optional) Additional check to ensure the current user owns this cart item
            string currentUserId = User.Identity.GetUserId();
            var currentCustomer = db.Customers.FirstOrDefault(c => c.ApplicationUserId == currentUserId);
            if (currentCustomer == null || cartItem.CustomerId != currentCustomer.CustomerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden); // Or any other handling you see fit
            }

            db.Carts.Remove(cartItem);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Cart/Checkout
        public ActionResult Checkout()
        {
            string userId = User.Identity.GetUserId();
            var customer = db.Customers.FirstOrDefault(c => c.ApplicationUserId == userId);
            if (customer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = db.Carts
                              .Include(c => c.Product)
                              .Where(c => c.CustomerId == customer.CustomerId)
                              .ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Your cart is empty."; // Using TempData is better here for messages after a redirect
                return RedirectToAction("Index");
            }

            // Setup ViewBag to display payment modes on the page
            ViewBag.PaymentMode = new SelectList(Enum.GetValues(typeof(PaymentMode)));
            return View(); // Return the view for the checkout page
        }

        // POST: Cart/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(string paymentMode) // Variable name must match the field name in the form
        {
            string userId = User.Identity.GetUserId();
            var customer = db.Customers.FirstOrDefault(c => c.ApplicationUserId == userId);
            if (customer == null) return RedirectToAction("Login", "Account");

            var cartItems = db.Carts
                              .Include(c => c.Product)
                              .Where(c => c.CustomerId == customer.CustomerId)
                              .ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Cart is empty!";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                CustomerId = customer.CustomerId,
                // CartId = cartItems.First().CartId, // Ensure your Order model has a CartId property if you want to save it, otherwise delete this line
                OrderDate = DateTime.Now,
                ShippingDate = DateTime.Now.AddDays(3), // You can make this more dynamic
                OrderStatus = OrderStatus.Processing, // Ensure OrderStatus is an enum you have defined
                OrderAmount = cartItems.Sum(c => c.GrandTotal)
            };
            db.Orders.Add(order);
            db.SaveChanges(); // Save the order to get the OrderId

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.ItemTotal,
                    MRP = item.Product.MRP
                };
                db.OrderItems.Add(orderItem);
            }

            db.Payments.Add(new Payment
            {
                CustomerId = customer.CustomerId,
                OrderId = order.OrderId,
                PaymentMode = (PaymentMode)Enum.Parse(typeof(PaymentMode), paymentMode),
                DateOfPayment = DateTime.Now
            });

            db.Carts.RemoveRange(cartItems);
            db.SaveChanges(); // Save OrderItems, Payment, and remove cart items

            TempData["Success"] = $"Order #{order.OrderId} placed successfully!";
            return RedirectToAction("Index"); // Or to an order confirmation page
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
