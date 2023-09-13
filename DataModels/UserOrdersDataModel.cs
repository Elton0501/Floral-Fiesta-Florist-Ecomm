using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class UserOrdersDataModel
    {
        public IEnumerable<OrderItems> CompleteOrderItems{ get; set; }
        public IEnumerable<OrderItems> PendingOrderItems { get; set; }
        public string Status { get; set; }
    }
}
