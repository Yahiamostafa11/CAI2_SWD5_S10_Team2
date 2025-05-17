using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ECommerceMVC.Models;

namespace ECommerceMVC.Controllers
{
    public class ProductsController : Controller
    {
        private ECommerceDbContext db = new ECommerceDbContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Seller);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            // Check if Categories exist
            if (!db.Categories.Any())
            {
                // Create a default category if none exist
                var category = new Category
                {
                    CategoryName = "Default Category"
                    // Add other required properties based on your Category model
                };
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["Message"] = "A default category was created because none existed.";
            }

            // Check if Sellers exist
            if (!db.Sellers.Any())
            {
                // Create a default seller if none exist
                var seller = new Seller
                {
                    Name = "Default Seller",
                    Phone = "123-456-7890",
                    TotalSales = 0
                };
                db.Sellers.Add(seller);
                db.SaveChanges();
                TempData["Message"] = "A default seller was created because none existed.";
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.SellerId = new SelectList(db.Sellers, "SellerId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,ProductName,MRP,Stock,Brand,CategoryId,SellerId")] Product product)
        {
            // Validate that CategoryId exists
            bool categoryExists = db.Categories.Any(c => c.CategoryId == product.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError("CategoryId", "The selected category does not exist.");
            }

            // Validate that SellerId exists
            bool sellerExists = db.Sellers.Any(s => s.SellerId == product.SellerId);
            if (!sellerExists)
            {
                ModelState.AddModelError("SellerId", "The selected seller does not exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "Unable to save the product. " + ex.Message);
                }
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.SellerId = new SelectList(db.Sellers, "SellerId", "Name", product.SellerId);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.SellerId = new SelectList(db.Sellers, "SellerId", "Name", product.SellerId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,ProductName,MRP,Stock,Brand,CategoryId,SellerId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.SellerId = new SelectList(db.Sellers, "SellerId", "Name", product.SellerId);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Helper method to create a default seller
        public ActionResult CreateDefaultSeller()
        {
            if (db.Sellers.Any())
            {
                TempData["Message"] = "Sellers already exist in the database.";
                return RedirectToAction("Index");
            }

            var seller = new Seller
            {
                Name = "Default Seller",
                Phone = "123-456-7890",
                TotalSales = 0
            };

            db.Sellers.Add(seller);
            db.SaveChanges();

            TempData["Message"] = "Default seller created successfully!";
            return RedirectToAction("Index");
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
