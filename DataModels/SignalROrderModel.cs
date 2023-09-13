using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class SignalROrderModel
    {
        public string CurrentOrderId { get; set; }
        public int TotalUser { get; set; }
        public int TotalProduct { get; set; }
        public int TodayOrdersCount { get; set; }
        public string TodayEarning { get; set; }
        public int MonthlyOrdersCount { get; set; }
        public string MonthlyEarning { get; set; }
        public int YearlyOrdersCount { get; set; }
        public string YearlyEarning { get; set; }
    }
}
