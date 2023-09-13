using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class OrderDetailsViewModel
    {
        public Orders Order { get; set; }
        public IEnumerable<OrderItems> OrderItems { get; set; }
    }

    public class OrdersViewModel
    {
        public IEnumerable<Orders> Orders { get; set; }
        public List<OrderStatusViewModel> OrderStatusEnum { get; set; }
        public List<PaymentStatusViewModel> PaymentStatusEnum { get; set; }
    }
    public class OrderStatusViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
    public class PaymentStatusViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
