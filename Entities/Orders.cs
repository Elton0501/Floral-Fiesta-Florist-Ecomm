using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required]
        public int createdBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public string razorpayKey { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string rzp_orderid { get; set; }
        [Required]
        public string ReceiverName { get; set; }
        [Required]
        public string ReceiverMobileNumber { get; set; }
        [Required]
        public string ReceiverEmail { get; set; }
        [Required]
        public string ShippingAddress { get; set; }
        [Required]
        public string PinCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int PaymentType { get; set; }
        [Required]
        public int PaymentStatus { get; set; }
        [Required]
        public int OrderStatus { get; set; }
        [NotMapped]
        public string OrderStatusName { get; set; }
        [Required]
        public Decimal BillAmount { get; set; }
        [Required]
        public int DeliveryCharges { get; set; }
        public string DeliveredDate { get; set; }
        [Required]
        public Decimal TotalAmount { get; set; }
        public virtual List<OrderItems> OrderItems { get; set; }
        [NotMapped]
        public User User { get; set; }
        [NotMapped]
        public string OrderStatusString { get; set; }
        [NotMapped]
        public string PaymentStatusString { get; set; }
        [NotMapped]
        public string PaymentTypeString { get; set; }
    }

}
