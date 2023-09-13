using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class BestSellersDataModel
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public Product Product { get; set; }
    }
}
