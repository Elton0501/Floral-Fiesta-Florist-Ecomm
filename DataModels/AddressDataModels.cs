using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class AddressDataModels
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string HouseNo { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string LandMark { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string DeliveryCharges { get; set; }
        [NotMapped]
        public string PaymentType { get; set; }
        [NotMapped]
        [Required]
        public string MobileNumber { get; set; }
        [NotMapped]
        [Required]
        public string Name { get; set; }
        [NotMapped]
        [Required, MinLength(1), DataType(DataType.EmailAddress), EmailAddress, MaxLength(50), Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [NotMapped]
        public bool DefaultAddress { get; set; }
        public int ItemCount { get; set; }
        public Decimal TotalAmount { get; set; }
    }
}
