using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Constant
    {
        public const string WebCookie = "FloralFiestaID";
        public const int ShippingDays = 6;
        public static class Role
        {
            public const string Admin = "Admin";
            public const string User = "User";
        }
        public static class NotificationConstant
        {
            public const string read = "Read";
            public const string unread = "Unread";
            public const string AddNewUser = "New user Register, User name is {0}, Email is {1}.";
            public const string AddNewOrder = "New order placed, order Id is {0}.";
        }
        public enum ShipmentCity
        {
            Thane = 1,
            Mumbai = 2,
            NewMumbai = 3,
        }
        public enum PaymentTypeEnum
        {
            COD = 0,
            Online = 1,
        }
        public enum PaymentStatusEnum
        {
            Pending = 0,
            Complete = 1
        }
        public enum OrderStatusEnum
        {
            Pending = 0,
            Processing = 1,
            Ready = 2,
            Dispatched = 3,            
            Complete = 4,
        }
    }
}
