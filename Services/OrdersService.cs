using DataModels;
using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using static DataModels.Constant;
using System.Security.Cryptography;

namespace Services
{
    public class OrdersService
    {
        #region singleton
        public static OrdersService Instance
        {
            get
            {
                if (instance == null) instance = new OrdersService();
                return instance;
            }
        }
        private static OrdersService instance { get; set; }

        public OrdersService()
        {
        }
        #endregion

        public string SaveOrder(AddressDataModels address)
        {
            var result = "false";
            using (var context = new SVIContext())
            {
                Orders orders = new Orders();
                orders.ShippingAddress = address.HouseNo + "," + address.LandMark + "," + address.Street + "," + address.City + "," + address.State + "," + address.Pincode;
                orders.ReceiverName = address.Name;
                orders.ReceiverMobileNumber = address.MobileNumber;
                orders.ReceiverEmail = address.Email;
                orders.PinCode = address.Pincode;
                orders.City = address.City;
                orders.PaymentType = Convert.ToInt32(address.PaymentType);
                //PaymentStatus for COD => Pending
                orders.PaymentStatus = Convert.ToInt32(Constant.PaymentStatusEnum.Pending);
                orders.OrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Pending);
                orders.DeliveryCharges = Convert.ToInt32(address.DeliveryCharges);
                orders.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                //it is user unique ID
                orders.createdBy = address.UserId;

                var user = UserService.Instance.GetUserByUID(address.UserId);
                var userCartData = CartService.Instance.GetListUserCart(user.Email);

                orders.BillAmount = userCartData.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum();
                orders.TotalAmount = orders.BillAmount + orders.DeliveryCharges;
                orders.OrderItems = userCartData.Select(x => new OrderItems()
                {
                    ItemId = x.ProductId,
                    Name = x.Product.Name,
                    Price = x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price,
                    Quantity = x.Quantity,
                    DDate = x.DDate,
                    SlotId = x.DSlot,
                    IDesc= x.Description,
                }).ToList();
                context.Orders.Add(orders);
                //Removed from Cart
                var RemoveCart = new List<Cart>();
                RemoveCart = context.Cart.Where(x => x.CreatedBy == user.Email).ToList();
                context.Cart.RemoveRange(RemoveCart);
                context.SaveChanges();
                result = orders.OrderId.ToString();
            }
            return result;
        }

        public Orders GetOrderDetails(AddressDataModels address)
        {
            using (var context = new SVIContext())
            {
                Orders orders = new Orders();
                orders.ShippingAddress = address.HouseNo + "," + address.LandMark + "," + address.Street + "," + address.City + "," + address.State + "," + address.Pincode;
                orders.ReceiverName = address.Name;
                orders.ReceiverMobileNumber = address.MobileNumber;
                orders.ReceiverEmail = address.Email;
                orders.PinCode = address.Pincode;
                orders.City = address.City;
                orders.PaymentType = Convert.ToInt32(address.PaymentType);
                //PaymentStatus for COD => Pending
                orders.PaymentStatus = Convert.ToInt32(Constant.PaymentStatusEnum.Pending);
                orders.OrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Pending);
                orders.DeliveryCharges = Convert.ToInt32(address.DeliveryCharges);
                orders.CreatedOn = DateTime.Now;
                //it is user unique ID
                orders.createdBy = address.UserId;

                var user = UserService.Instance.GetUserByUID(address.UserId);
                var userCartData = CartService.Instance.GetListUserCart(user.Email);
                orders.User = user;
                orders.BillAmount = userCartData.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum();
                orders.TotalAmount = orders.BillAmount + orders.DeliveryCharges;
                return orders;
            }

        }

        public IEnumerable<Orders> GetAllOrders()
        {
            using(var context = new SVIContext())
            {
                return context.Orders.ToList();
            }
        }
        public IEnumerable<OrderItems> GetAllOrderItems()
        {
            using(var context = new SVIContext())
            {
                return context.OrderItems.ToList();
            }
        }

        public IEnumerable<Product> GetBestSellers(IEnumerable<Product> product)
        {
            using (var context = new SVIContext())
            {
                var Plist = new List<Product>();
                var list = new List<BestSellersDataModel>();
                var orderItems = context.OrderItems.ToList();
                list = orderItems.Select(x => new BestSellersDataModel()
                {
                    ProductId = x.ItemId,
                    Count = orderItems.Where(z => z.ItemId == x.ItemId).Count(),
                    Product = product.FirstOrDefault(z => z.Id == x.ItemId),
                }).ToList();
                list = list.Select(x =>
                {
                    x.Product.BestSellerCount = x.Count;
                    Plist.Add(x.Product);
                    return x;
                }).ToList();
                return Plist;
            }
        }

        public IEnumerable<Orders> GetPendingOrders(string[] OrderStatus, int? PaymentStatus, DateTime? StartDate, DateTime? EndDate)
        {
            using(var context = new SVIContext())
            {
                int pendingStatus = Convert.ToInt32(Constant.OrderStatusEnum.Pending);
                int DispatchedStatus = Convert.ToInt32(Constant.OrderStatusEnum.Dispatched);
                int ProcessingStatus = Convert.ToInt32(Constant.OrderStatusEnum.Processing);
                int ShippedStatus = Convert.ToInt32(Constant.OrderStatusEnum.Ready);
                var data = context.Orders.Include(x=>x.OrderItems).Where(x => x.OrderStatus == pendingStatus
                || x.OrderStatus == DispatchedStatus 
                || x.OrderStatus == ProcessingStatus
                || x.OrderStatus == ShippedStatus).ToList();
                data = data.Select(x =>
                {
                    x.User = UserService.Instance.GetUserByUID(x.createdBy);
                    x.PaymentStatusString = Constant.PaymentStatusEnum.GetName(typeof(Constant.PaymentStatusEnum), x.PaymentStatus);
                    x.PaymentTypeString = Constant.PaymentStatusEnum.GetName(typeof(Constant.PaymentTypeEnum), x.PaymentType);
                    x.OrderStatusString = Constant.PaymentStatusEnum.GetName(typeof(Constant.OrderStatusEnum), x.OrderStatus);
                    return x;
                }).OrderByDescending(x=>x.OrderId).ToList();
                if (OrderStatus != null)
                {
                    var newOrder =new List<Orders>();
                    for (int i = 0; i < OrderStatus.Length; i++)
                    {
                        int darray = Convert.ToInt32(OrderStatus[i]);
                        var olddata = data.Where(x => x.OrderStatus == darray).ToList();
                        newOrder.AddRange(olddata);
                    }
                    data = newOrder;
                }
                if (PaymentStatus.HasValue && PaymentStatus.Value >= 0)
                {
                    data = data.Where(x => x.PaymentStatus == PaymentStatus).ToList();
                }
                if (StartDate.HasValue && StartDate != null)
                {
                    StartDate = Convert.ToDateTime(StartDate.Value.ToString("dd-MM-yyyy"));
                    data = data.Where(x => x.CreatedOn >= StartDate).ToList();
                }
                if (EndDate.HasValue && EndDate != null)
                {
                    EndDate = Convert.ToDateTime(EndDate.Value.ToString("dd-MM-yyyy"));
                    data = data.Where(x => x.CreatedOn <= EndDate).ToList();
                }
                return data;
            }
        }

        public List<OrderItems> GetUserCurrentOrder(int orderId)
        {
            using(var context = new SVIContext())
            {
                var list = new List<OrderItems>();
                var products = context.Products.Where(x => x.Status == true).Include(x => x.ProductImages).ToList();
                int completeOrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Complete);
                int completepayStatus = Convert.ToInt32(Constant.PaymentStatusEnum.Complete);
                var data = context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.OrderId == orderId);
                list = data.OrderItems;
                list = list.Select(x =>
                {
                    x.Orders = data;
                    x.Product = products.FirstOrDefault(y => y.Id == x.ItemId);
                    x.DefaultImage = x.Product.ProductImages.Count > 0 ? x.Product.ProductImages.FirstOrDefault(a => a.Default == true) != null ? x.Product.ProductImages.FirstOrDefault(a => a.Default == true).Image : x.Product.ProductImages.FirstOrDefault().Image : "";
                    return x;
                }).OrderByDescending(x => x.Id).ToList();
                return list;
            }
        }

        public IEnumerable<OrderItems> GetUserPenOrderItemsList(int Id)
        {
            using (var context = new SVIContext())
            {
                //IEnumerable<OrderItems> list = null;
                var list = new List<OrderItems>();
                var products = context.Products.Where(x => x.Status == true).Include(x => x.ProductImages).ToList();
                int pendingStatus = Convert.ToInt32(Constant.OrderStatusEnum.Pending);
                int DispatchedStatus = Convert.ToInt32(Constant.OrderStatusEnum.Dispatched);
                int ProcessingStatus = Convert.ToInt32(Constant.OrderStatusEnum.Processing);
                int ShippedStatus = Convert.ToInt32(Constant.OrderStatusEnum.Ready);
                var data = context.Orders.Include(x => x.OrderItems).Where(x => x.OrderStatus == pendingStatus
                  || x.OrderStatus == DispatchedStatus
                  || x.OrderStatus == ProcessingStatus
                  || x.OrderStatus == ShippedStatus && x.createdBy == Id).ToList();
                data = data.Select(x =>
                {
                    list.AddRange(x.OrderItems);
                    return x;
                }).OrderByDescending(x => x.OrderId).ToList();
                list = list.Select(x =>
                {
                    x.Orders.OrderStatusName = Enum.GetName(typeof(Constant.OrderStatusEnum), x.Orders.OrderStatus).ToString();
                    x.Product = products.FirstOrDefault(y => y.Id == x.ItemId);
                    x.DefaultImage = x.Product.ProductImages.Count > 0 ? x.Product.ProductImages.FirstOrDefault(a => a.Default == true) != null ? x.Product.ProductImages.FirstOrDefault(a => a.Default == true).Image : x.Product.ProductImages.FirstOrDefault().Image : "";
                    return x;
                }).OrderByDescending(x => x.Id).ToList();
                return list;
            }
        }
        public IEnumerable<Orders> GetCompleteOrdersList(DateTime? StartDate, DateTime? EndDate)
        {
            using(var context = new SVIContext())
            {
                int completeOrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Complete);
                var data = context.Orders.Include(x => x.OrderItems).Where(x => x.OrderStatus == completeOrderStatus).ToList();
                data = data.Select(x =>
                {
                    x.User = UserService.Instance.GetUserByUID(x.createdBy);
                    x.PaymentStatusString = Constant.PaymentStatusEnum.GetName(typeof(Constant.PaymentStatusEnum), x.PaymentStatus);
                    x.PaymentTypeString = Constant.PaymentStatusEnum.GetName(typeof(Constant.PaymentTypeEnum), x.PaymentType);
                    x.OrderStatusString = Constant.PaymentStatusEnum.GetName(typeof(Constant.OrderStatusEnum), x.OrderStatus);
                    return x;
                }).ToList();
                if (StartDate.HasValue && StartDate != null)
                {
                    StartDate = Convert.ToDateTime(StartDate.Value.ToString("dd-MM-yyyy"));
                    data = data.Where(x => x.CreatedOn >= StartDate).ToList();
                }
                if (EndDate.HasValue && EndDate != null)
                {
                    EndDate = Convert.ToDateTime(EndDate.Value.ToString("dd-MM-yyyy"));
                    data = data.Where(x => x.CreatedOn <= EndDate).ToList();
                }
                return data;
            }
        }
        public IEnumerable<OrderItems> GetUserComOrdersList(int Id)
        {
            using (var context = new SVIContext())
            {
                var list = new List<OrderItems>();
                var products = context.Products.Where(x => x.Status == true).Include(x=>x.ProductImages).ToList();
                int completeOrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Complete);
                int completepayStatus = Convert.ToInt32(Constant.PaymentStatusEnum.Complete);
                var data = context.Orders.Include(x => x.OrderItems).Where(x => x.OrderStatus == completeOrderStatus && x.PaymentStatus == completepayStatus && x.createdBy == Id).ToList();
                data = data.Select(x =>
                {
                    list.AddRange(x.OrderItems);
                    return x;
                }).ToList();
                list = list.Select(x =>
                {
                    x.Product = products.FirstOrDefault(y => y.Id == x.ItemId);
                    x.DefaultImage = x.Product.ProductImages.Count > 0 ? x.Product.ProductImages.FirstOrDefault(a => a.Default == true) != null ? x.Product.ProductImages.FirstOrDefault(a => a.Default == true).Image : x.Product.ProductImages.FirstOrDefault().Image : "";
                    return x;
                }).OrderByDescending(x=>x.Id).ToList();
                return list;
            }
        }
        public IEnumerable<OrderItems> GetOrderItems(int orderID)
        {
            using(var context = new SVIContext())
            {
                return context.OrderItems.Where(x => x.OrderId == orderID).ToList();
            }
        }

        public Orders GetOrderDetails(int orderID)
        {
            using (var context = new SVIContext())
            {
                var data = context.Orders.Include(x => x.OrderItems).FirstOrDefault(x => x.OrderId == orderID);
                data.User = UserService.Instance.GetUserByUID(data.createdBy);
                data.PaymentStatusString = Constant.PaymentStatusEnum.GetName(typeof(Constant.PaymentStatusEnum), data.PaymentStatus);
                data.PaymentTypeString = Constant.PaymentStatusEnum.GetName(typeof(Constant.PaymentTypeEnum), data.PaymentType);
                data.OrderStatusString = Constant.PaymentStatusEnum.GetName(typeof(Constant.OrderStatusEnum), data.OrderStatus);
                return data;
            }
        }

        public string OrderStatusChanged(int orderID, int status)
        {
            var result = "false";
            using (var context = new SVIContext())
            {
                var data = context.Orders.FirstOrDefault(x=>x.OrderId == orderID);
                data.OrderStatus = status;
                if (status == Convert.ToInt32(Constant.OrderStatusEnum.Complete))
                {
                    data.DeliveredDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).ToString();
                }
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = "true";
                return result;
            }
        }
        public string OrderPaymentChanged(int orderID, int status)
        {
            var result = "false";
            using (var context = new SVIContext())
            {
                var data = context.Orders.FirstOrDefault(x=>x.OrderId == orderID);
                data.PaymentStatus = status;
                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();
                result = "true";
                return result;
            }
        }

        public string SlotTime(string sId)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    return context.Keys.FirstOrDefault(x => x.Type == "Slot" && x.Name == sId).Description; 
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int getOrdersCount()
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var count = context.Orders.ToList();
                    return count != null ? count.Count() + 1 : 1;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
