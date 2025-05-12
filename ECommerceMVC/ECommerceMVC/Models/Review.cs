using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceMVC.Models {
    public enum Rating
    {
        One = 1,
        Two,
        Three,
        Four,
        Five
    }

    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public string Description { get; set; }
        public Rating Rating { get; set; }

        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

}
