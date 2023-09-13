using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class CartDataModel
    {
        public string Count { get; set; }
        public Decimal TotalwithoutDisc { get; set; }
        public Decimal TotalAmount { get; set; }
        public bool result { get; set; }
        public string isLogin { get; set; }
        public string CartId { get; set; }
    }

    public class CartView
    {
        public IEnumerable<Cart> Carts { get; set; }
        public Decimal TotalwithoutDisc { get; set; }
        public Decimal TotalAmount { get; set; }
    }
}
