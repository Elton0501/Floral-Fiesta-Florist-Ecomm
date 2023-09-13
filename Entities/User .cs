using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Varified { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid RoleId { get; set; }
        public List<Address> Addresses { get; set; }

        [NotMapped]
        public string HouseNo { get; set; }
        [NotMapped]
        public string Street { get; set; }
        [NotMapped]
        public string LocalArea { get; set; }
        [NotMapped]
        public string LandMark { get; set; }
        [NotMapped]
        public string Pincode { get; set; }
        [NotMapped]
        public string Country { get; set; }
        [NotMapped]
        public string State { get; set; }
        [NotMapped]
        public string City { get; set; }
        [NotMapped]
        public string PaymentType { get; set; }
    }
}
