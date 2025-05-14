using ECommerceMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;


public class ApplicationUser : IdentityUser
{
    public virtual Customer Customer { get; set; }

    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
        var identity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
        return identity;
    }
}


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext() : base("ECommerceConnection") { }

    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }

    public DbSet<Customer> Customers { get; set; }
}
