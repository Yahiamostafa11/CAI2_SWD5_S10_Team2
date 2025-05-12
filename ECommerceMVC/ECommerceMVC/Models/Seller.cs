using ECommerceMVC.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Models
{
    public class Seller
    {
        [Key]
        public int SellerId { get; set; }

        public string Name { get; set; }

        [Phone]
        public string Phone { get; set; }

        public float TotalSales { get; set; }

        // Navigation Property
        public virtual ICollection<Product> Products { get; set; }
    }

}
