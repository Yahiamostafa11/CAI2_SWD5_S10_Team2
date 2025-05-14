using System.Linq;
using System.Web.Mvc;
using ECommerceMVC.Models;

namespace ECommerceMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ECommerceDbContext db = new ECommerceDbContext();

        public ActionResult Dashboard()
        {
            ViewBag.TotalUsers = db.Customers.Count();
            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalSales = db.Orders.Sum(o => (decimal?)o.OrderAmount) ?? 0;
            ViewBag.TotalProducts = db.Products.Count();

            return View();
        }
    }
}
