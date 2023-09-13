using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DDate { get; set; }
        public string DSlot { get; set; }
        public string  Description { get; set; }
        public string CreatedBy { get; set; }
        [NotMapped]
        public Product Product { get; set; }

    }
}
