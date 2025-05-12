using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


 
namespace ECommerceMVC.Models {
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public int ApartNo { get; set; }
        public string ApartName { get; set; }
        public string StreetName { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public int Pincode { get; set; }

        // Foreign Key
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
