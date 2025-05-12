using ECommerceMVC.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Models {
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
        public string Description { get; set; }

        // Navigation Property
        public virtual ICollection<Product> Products { get; set; }
    }
}

