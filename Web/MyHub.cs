using DataModels;
using Entities;
using Microsoft.AspNet.SignalR;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public class MyHub : Hub
    {
        public void TotalOrderData(string OrderId)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            //Get TotalNotification  
            var data = new SignalROrderModel();
            data = LoadTotalOrderData(OrderId);
            context.Clients.All.broadcaastTotalOrderData(data);
        }
        public SignalROrderModel LoadTotalOrderData(string OrderId)
        {
            var data = new SignalROrderModel();
            data.TodayOrdersCount = 0;
            data.MonthlyOrdersCount = 0;
            data.YearlyOrdersCount = 0;
            data.TodayEarning = "0.00";
            data.MonthlyEarning = "0.00";
            data.YearlyEarning = "0.00";
            data.CurrentOrderId = OrderId;
            DateTime Today = DateTime.Now;
            using (var context = new SVIContext())
            {
                var TotalOrders = (from t in context.Orders select t).ToList();
                var query1 = (from t in TotalOrders where t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month && t.CreatedOn.Day == Today.Day select t).ToList();
                var query2 = (from t in TotalOrders where t.CreatedOn.Year == Today.Year && t.CreatedOn.Month == Today.Month select t).ToList();
                var query3 = (from t in TotalOrders where t.CreatedOn.Year == Today.Year select t).ToList();
                data.TodayOrdersCount = query1.Count();
                data.MonthlyOrdersCount = query2.Count();
                data.YearlyOrdersCount = query3.Count();
                data.TodayEarning = query1.Count > 0 ? query1.Select(x => x.TotalAmount).Sum().ToString("f") : "0.00";
                data.MonthlyEarning = query2.Count > 0 ? query2.Select(x => x.TotalAmount).Sum().ToString("f") : "0.00";
                data.YearlyEarning = query3.Count > 0 ? query3.Select(x => x.TotalAmount).Sum().ToString("f") : "0.00";
                return data;
            }
        }
        public void NotificationBrodcaast(Notification notification)
        {
            if (notification.Message != null && notification.Message != string.Empty)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                //Get TotalNotification  
                context.Clients.All.BrodcaastNotificationData();
            }
        }
    }
}