using System.Collections.Generic;
using System.Data.Entity;

namespace ECommerceMVC.Models {
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext() : base("name=ECommerceConnection") { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // منع الحذف المتتالي للعلاقة بين Order و Customer
            // هذا يفترض أن لديك خاصية Customer في نموذج Order
            // وخاصية Orders (مجموعة) في نموذج Customer
            // وأن المفتاح الأجنبي في Order هو CustomerId
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer) // إذا كانت العلاقة إلزامية
                                              // .HasOptional(o => o.Customer) // إذا كانت العلاقة اختيارية
                .WithMany(c => c.Orders)      // افترض أن Customer لديه خاصية ICollection<Order> Orders
                .HasForeignKey(o => o.CustomerId)
                .WillCascadeOnDelete(false); // هذا هو الجزء المهم لتعطيل الحذف المتتالي

            // قد تحتاج إلى إضافة تكوينات مشابهة لعلاقات أخرى إذا تسببت في مشاكل مماثلة

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
            .HasRequired(oi => oi.Product) // إذا كانت العلاقة إلزامية
                                      // .HasOptional(oi => oi.Product) // إذا كانت العلاقة اختيارية
            .WithMany() // إذا لم يكن لدى Product خاصية ICollection<OrderItem> OrderItems، يمكنك ترك .WithMany() فارغة
                   // أو إذا كانت موجودة، استخدم .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .WillCascadeOnDelete(false); // هذا هو الجزء المهم لتعطيل الحذف المتتالي

            modelBuilder.Entity<Payment>()
            .HasRequired(p => p.Order) // إذا كانت العلاقة إلزامية
                                  // .HasOptional(p => p.Order) // إذا كانت العلاقة اختيارية
            .WithMany(o => o.Payments) // افترض أن Order لديه خاصية ICollection<Payment> Payments
                                  // إذا لم يكن لدى Order خاصية Payments، يمكنك استخدام .WithMany() فارغة
            .HasForeignKey(p => p.OrderId)
             .WillCascadeOnDelete(false);
        }
    }


}

