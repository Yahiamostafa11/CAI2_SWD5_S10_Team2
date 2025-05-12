using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceMVC.Models
{
    public enum PaymentMode
    {
        Cash,
        CreditCard,
        DebitCard,
        NetBanking,
        UPI
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public PaymentMode PaymentMode { get; set; }
        public DateTime DateOfPayment { get; set; }

        public int CustomerId { get; set; }
        public int OrderId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }

}
