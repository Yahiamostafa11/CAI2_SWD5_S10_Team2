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
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                // 1. Create Admin role if not exists
                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    roleManager.Create(role);
                }

                // 2. Create new admin user
                var adminEmail = "yahiawork11@gmail.com";
                var adminPassword = "12345";  // ✅ SET YOUR OWN PASSWORD HERE

                var adminUser = userManager.FindByEmail(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail
                    };
                    var result = userManager.Create(adminUser, adminPassword);

                    if (result.Succeeded)
                    {
                        userManager.AddToRole(adminUser.Id, "Admin");
                    }
                }
                else if (!userManager.IsInRole(adminUser.Id, "Admin"))
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }
        }

    }
}
