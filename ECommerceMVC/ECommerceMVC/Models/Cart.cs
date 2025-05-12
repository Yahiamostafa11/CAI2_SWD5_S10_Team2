using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceMVC.Models {
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public float GrandTotal { get; set; }
        public int ItemTotal { get; set; }

        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
