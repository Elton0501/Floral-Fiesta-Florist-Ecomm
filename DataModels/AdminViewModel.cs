using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class AdminViewModel
    {
        public int TotalUser { get; set; }
        public int TotalProduct { get; set; }
        public int TodayOrdersCount { get; set; }
        public decimal TodayEarning { get; set; }
        public int MonthlyOrdersCount { get; set; }
        public decimal MonthlyEarning { get; set; }
        public int YearlyOrdersCount { get; set; }
        public decimal YearlyEarning { get; set; }
        public List<UserOrdersCount> userOrdersCounts { get; set; }
        public List<OrderItemsCount> OrderItemsCount { get; set; }

    }
}
