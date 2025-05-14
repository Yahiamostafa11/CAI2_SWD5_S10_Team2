using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework; // Ensure this is present for IdentityRole, RoleStore, UserStore
using ECommerceMVC.Models; // Ensure this is present for ApplicationDbContext, ApplicationUser
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ECommerceMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CreateAdminRoleAndUser();
        }

        // The CreateAdminRoleAndUser method should be part of the MvcApplication class
        private void CreateAdminRoleAndUser()
        {
            // Make sure you have the correct ApplicationDbContext. 
            // If your DbContext for Identity is different, use that one.
            using (var context = new ApplicationDbContext()) // Or new ECommerceDbContext() if that's your Identity context
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

                // Create Admin role if it doesn't exist
                string adminRoleName = "Admin";
                if (!roleManager.RoleExists(adminRoleName))
                {
                    var role = new IdentityRole(adminRoleName);
                    roleManager.Create(role);
                }

                // Assign admin user to Admin role
                // Ensure the user "Yahiawork11@gmail.com" exists in your database before running this.
                // This code assumes the user is already created (e.g., through a registration page).
                string adminUserEmail = "Yahiawork11@gmail.com";
                var adminUser = userManager.FindByEmail(adminUserEmail);
                if (adminUser != null)
                {
                    if (!userManager.IsInRole(adminUser.Id, adminRoleName))
                    {
                        userManager.AddToRole(adminUser.Id, adminRoleName);
                    }
                }
                else
                {
                    // Optional: Handle case where admin user doesn't exist.
                    // You might want to log this, or even create the user here if that's part of your startup logic.
                    // For example (ensure you have a password and other required fields for ApplicationUser):
                    // var newUser = new ApplicationUser { UserName = adminUserEmail, Email = adminUserEmail, EmailConfirmed = true };
                    // var result = userManager.Create(newUser, "YourAdminPasswordHere"); // Replace with a strong password
                    // if (result.Succeeded)
                    // {
                    //    userManager.AddToRole(newUser.Id, adminRoleName);
                    // }
                }
            }
        }
    }
}
