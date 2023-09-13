using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public DateTime Create { get; set; }
        public int ProductId { get; set; }
        public bool Default { get; set; }
        public virtual Product Product { get; set; }
    }
}
