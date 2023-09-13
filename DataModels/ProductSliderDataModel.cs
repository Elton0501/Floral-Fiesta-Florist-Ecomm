using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class ProductSliderDataModel
    {
        public IEnumerable<Product> Product { get; set; }
        public int ProductCount { get; set; }
    }
}
