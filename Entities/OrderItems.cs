using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Decimal Price { get; set; }
        public virtual Orders Orders { get; set; }
        public string  SlotId { get; set; }
        public string DDate { get; set; }
        public string IDesc { get; set; }
        [NotMapped]
        public Key key { get; set; }
        [NotMapped]
        public Product Product { get; set; }
        [NotMapped]
        public string DefaultImage { get; set; }
        [NotMapped]
        public int? UserID { get; set; }
    }
}
