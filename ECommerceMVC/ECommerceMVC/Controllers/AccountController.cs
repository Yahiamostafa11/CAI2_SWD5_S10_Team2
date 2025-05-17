using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ECommerceMVC.Models;
using System.Linq;

namespace ECommerceMVC.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;

        public AccountController()
        {
            _context = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            _context = new ApplicationDbContext();
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        // GET: /Account/Register
        public ActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Save Customer record linked to Identity user
                var customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    ApplicationUserId = user.Id
                };

                // Use ECommerceDbContext to save customer
                var ecommerceDb = new ECommerceDbContext();
                ecommerceDb.Customers.Add(customer);
                ecommerceDb.SaveChanges();

                // Sign in the user
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false },
                    await user.GenerateUserIdentityAsync(UserManager));

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await UserManager.FindAsync(model.Email, model.Password);
            if (user != null)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                var identity = await user.GenerateUserIdentityAsync(UserManager);
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

                // Redirect to returnUrl if provided, otherwise to Home/Index
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // Helper method to redirect to local URL or default
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/CreateAdmin
        [Authorize] // Restrict access to authenticated users
        public ActionResult CreateAdmin()
        {
            // This is a temporary action to create an admin user
            // In production, you would want to secure this better
            return View();
        }

        // POST: /Account/CreateAdmin
        [HttpPost]
        [Authorize] // Restrict access to authenticated users
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAdmin(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required");
                return View();
            }

            // Find the user by email
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View();
            }

            // Check if Admin role exists, create if not
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_context));
            if (!roleManager.RoleExists("Admin"))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", "Error creating Admin role");
                    return View();
                }
            }

            // Add user to Admin role
            var result = await UserManager.AddToRoleAsync(user.Id, "Admin");
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home", new { message = "User added to Admin role successfully" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View();
        }
    }
}
