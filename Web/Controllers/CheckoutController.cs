using CCA.Util;
using DataModels;
using Entities;
using Microsoft.Ajax.Utilities;
using Razorpay.Api;
using Services;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Device.Location;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Checkout
        public ActionResult Address() 
        {
            try
            {
                var data = new AddressDataModels();
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser != null && loginUser != string.Empty)
                {
                    var user = UserService.Instance.GetUser(loginUser);
                    data.UserId = user.Id;
                    if (user != null && user.Id > 0)
                    {
                        var address = UserService.Instance.GetAddress(data.UserId);
                        if (address != null)
                        {
                            data.UserId = user.Id;
                            data.Name = user.Name;
                            data.Email = user.Email;
                            data.MobileNumber = user.MobileNumber;
                            data.HouseNo = address.HouseNo;
                            data.Street = address.Street;
                            data.Country = address.Country;
                            data.State = address.State;
                            data.Pincode = address.Pincode;
                            data.City = address.City;
                            data.LandMark = address.LandMark;
                        }
                    }
                    var cartdata = CartService.Instance.GetListUserCart(loginUser);
                    data.ItemCount = cartdata.Select(x => x.Quantity).Sum();
                    data.TotalAmount = cartdata.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum();
                    if (data.ItemCount == 0 || data.TotalAmount < 1)
                    {
                        return RedirectToAction("/Home/Index");
                    }
                }
                return View(data);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        [HttpPost]
        public ActionResult Address(AddressDataModels Address)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(Address);
                }
                var result = "false";
                if (Address.DefaultAddress == true)
                {
                    UserService.Instance.SaveAddress(Address);
                }
                if (Convert.ToInt32(Address.PaymentType) == Convert.ToInt32(Constant.PaymentTypeEnum.Online))
                {
                    //RazorPay
                    var order = OrdersService.Instance.GetOrderDetails(Address);
                    // Generate random receipt number for order
                    Random randomObj = new Random();
                    string transactionId = randomObj.Next(10000000, 100000000).ToString();

                    //Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(ConfigurationManager.AppSettings["KeyId"], ConfigurationManager.AppSettings["KeySecret"]);
                    //Dictionary<string, object> options = new Dictionary<string, object>();
                    //options.Add("amount", order.TotalAmount * 100);  // Amount will in paise
                    //options.Add("receipt", transactionId);
                    //options.Add("currency", "INR");
                    //options.Add("payment_capture", "0");
                    //// 0 - automatic  , 1 - manual                                                   
                    ////options.Add("notes", "-- You can put any notes here --");
                    //Razorpay.Api.Order orderResponse = client.Order.Create(options);
                    //order.rzp_orderid = orderResponse["id"].ToString();
                    //order.razorpayKey = ConfigurationManager.AppSettings["KeyId"];
                    //order.currency = "INR";
                    //order.description = "Payment";
                    //// Return on PaymentPage with Order data
                    //return View("PaymentPage", order);
                    //// return RedirectToAction("SaveOnlinePaymentOrder", new { address = Address });
                    ///
                    //ccavenue Paymetgateway
                    string WorkingKey = ConfigurationManager.AppSettings["CcAvenueWorkingKey"].ToString(); //put in the 32bit alpha numeric key in the quotes provided here
                    string Merchant_id = ConfigurationManager.AppSettings["CcAvenueMerchantId"].ToString();
                    string CheckoutUrl = ConfigurationManager.AppSettings["CcAvenueCheckoutUrl"].ToString();
                    string AccessCode = ConfigurationManager.AppSettings["CcAvenueAccessCode"].ToString();
                    string amount = Address.TotalAmount.ToString();
                    var queryParameter = new CCACrypto();
                    return View("CcAvenue", new CcAvenueViewModel(queryParameter.Encrypt(BuildCcAvenueRequestParameters(transactionId, amount,Address), WorkingKey), AccessCode, CheckoutUrl));


                    //CCACrypto ccaCrypto = new CCACrypto();
                    //
                    //string StrEncRequest = string.Empty;
                    //string ccaRequest = string.Empty;
                    // // put the access key in the quotes provided here.;
                    //var callBackUrl = Request.Url.ToString().Substring(0,
                    //          Request.Url.ToString().Length - Request.Url.PathAndQuery.Length +
                    //          Url.Content("~").Length) + "Return/Return";
                    //RemotePost remotepost = new RemotePost();
                    //FormCollection formCollection1 = new FormCollection
                    //{
                    //    ["merchant_id"] = Merchant_id,
                    //    ["order_id"] = "132132132", //Need to generate ramdom nuber for every request.
                    //    ["currency"] = Address.TotalAmount.ToString(),
                    //    ["redirect_url"] = callBackUrl,
                    //    ["cancel_url"] = callBackUrl,
                    //    ["tid"] = "21332132132", // uniqe number only numrics.

                    //    ["billing_name"] = Address.Name,
                    //    ["billing_address"] = Address.HouseNo + Address.LandMark + Address.City + Address.Country + Address.Pincode,
                    //    ["billing_city"] = Address.City,
                    //    ["billing_state"] = Address.State,
                    //    ["billing_zip"] = Address.Pincode,
                    //    ["billing_country"] = Address.Country,
                    //    ["billing_tel"] = Address.MobileNumber,
                    //    ["billing_email"] = Address.Email,

                    //    ["delivery_name"] = Address.Name,
                    //    ["delivery_address"] = Address.HouseNo + Address.LandMark + Address.City + Address.Country + Address.Pincode,
                    //    ["delivery_city"] = Address.City,
                    //    ["delivery_state"] = Address.State,
                    //    ["delivery_zip"] = Address.Pincode,
                    //    ["delivery_country"] = Address.Country,
                    //    ["delivery_tel"] = Address.MobileNumber,
                    //    ["delivery_email"] = Address.Email,

                    //    //["merchant_param1"] = formCollection["merchant_param1"],
                    //    //["merchant_param2"] = formCollection["merchant_param2"],
                    //    //["merchant_param3"] = formCollection["merchant_param3"],
                    //    //["merchant_param4"] = formCollection["merchant_param4"],
                    //    //["merchant_param5"] = formCollection["merchant_param5"]
                    //};
                    //remotepost.Url = CheckoutUrl;
                    //remotepost.Add("merchant_id", formCollection1["merchant_id"]);
                    //remotepost.Add("order_id", formCollection1["order_id"]);

                    //remotepost.Add("currency", formCollection1["currency"]);
                    //remotepost.Add("redirect_url", "/PaymentSuccessful");
                    //remotepost.Add("cancel_url", "/PaymentCancelled");
                    //remotepost.Add("tid", formCollection1["tid"]);
                    //remotepost.Add("billing_name", formCollection1["billing_name"]);
                    //remotepost.Add("billing_address", formCollection1["billing_address"]);
                    //remotepost.Add("billing_city", formCollection1["billing_city"]);
                    //remotepost.Add("billing_state", formCollection1["billing_state"]);
                    //remotepost.Add("billing_zip", formCollection1["billing_zip"]);
                    //remotepost.Add("billing_tel", formCollection1["billing_tel"]);
                    //remotepost.Add("billing_email", formCollection1["billing_email"]);

                    //remotepost.Add("delivery_name", formCollection1["delivery_name"]);
                    //remotepost.Add("delivery_address", formCollection1["delivery_address"]);
                    //remotepost.Add("delivery_city", formCollection1["delivery_city"]);
                    //remotepost.Add("delivery_state", formCollection1["delivery_state"]);
                    //remotepost.Add("delivery_zip", formCollection1["delivery_zip"]);
                    //remotepost.Add("delivery_tel", formCollection1["delivery_tel"]);
                    //remotepost.Add("delivery_email", formCollection1["delivery_email"]);

                    ////remotepost.Add("merchant_param1", formCollection1["merchant_param1"]);
                    ////remotepost.Add("merchant_param2", formCollection1["merchant_param2"]);
                    ////remotepost.Add("merchant_param3", formCollection1["merchant_param3"]);
                    ////remotepost.Add("merchant_param4", formCollection1["merchant_param4"]);
                    ////remotepost.Add("merchant_param5", formCollection1["merchant_param5"]);

                    //foreach (string name in formCollection1)
                    //{
                    //    if (name != null)
                    //    {
                    //        if (!name.StartsWith("_"))
                    //        {
                    //            ccaRequest = ccaRequest + name + "=" + formCollection1[name] + "&";
                    //        }
                    //    }
                    //}
                    //StrEncRequest = ccaCrypto.Encrypt(ccaRequest, WorkingKey);
                    //remotepost.Add("encRequest", StrEncRequest);
                    //remotepost.Add("access_code", StrAccessCode);
                    //remotepost.Post();
                }
                else
                {
                    result = OrdersService.Instance.SaveOrder(Address);
                    if (result != "false" && result != string.Empty)
                    {
                        var notification = new Notification();
                        notification.CreatedOn = DateTime.Now;
                        notification.isMultipleUser = false;
                        notification.Status = Constant.NotificationConstant.unread;
                        notification.AnchorLink = "/Admin/PendingOrdersList";
                        notification.Message = string.Format(Constant.NotificationConstant.AddNewOrder, result);
                        var notificationAction = new NotificationactionController();
                        notificationAction.SaveNotification(notification);
                        MyHub objNotifHub = new MyHub();
                        objNotifHub.TotalOrderData(result);
                    }

                }
                return RedirectToAction("Thankyou", new { OrderId = result });
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        #region CCAvenue
        private string BuildCcAvenueRequestParameters(string invoiceNumber, string amount, AddressDataModels Address)
        {
            var orderid = OrdersService.Instance.getOrdersCount();
            string WorkingKey = ConfigurationManager.AppSettings["CcAvenueWorkingKey"].ToString(); //put in the 32bit alpha numeric key in the quotes provided here
            string Merchant_id = ConfigurationManager.AppSettings["CcAvenueMerchantId"].ToString();
            string CheckoutUrl = ConfigurationManager.AppSettings["CcAvenueCheckoutUrl"].ToString();

            var queryParameters = new Dictionary<string, string>
             {
             {"order_id", orderid.ToString()},
             {"merchant_id", Merchant_id},
             {"amount", amount},
             {"currency","INR" },
             {"redirect_url","https://floralfiesta.in/Checkout/PaymentSuccessful" },
             {"cancel_url","https://floralfiesta.in/Checkout/PaymentCancelled"},
             { "tid" , invoiceNumber }, // uniqe number only numrics.
             {"billing_name" , Address.Name},
             {"billing_address", Address.HouseNo + Address.LandMark + Address.City + Address.Country + Address.Pincode },
             {"billing_city", Address.City},
             {"billing_state", Address.State},
             {"billing_zip", Address.Pincode},
             {"billing_country", Address.Country},
             {"billing_tel", Address.MobileNumber},
             {"billing_email", Address.Email},
             {"delivery_name", Address.Name},
             {"delivery_address", Address.HouseNo},
             {"delivery_city", Address.City},
             {"delivery_state", Address.State},
             {"delivery_zip", Address.Pincode},
             {"delivery_country", Address.Country},
             {"delivery_tel", Address.MobileNumber},
             {"merchant_param1", Address.DeliveryCharges },
             {"merchant_param2", Address.UserId.ToString() },
             {"request_type","JSON" },
             {"response_type","JSON" },
             {"version","1.1" }
        }.Select(item => string.Format("{0}={1}", item.Key, item.Value));
            return string.Join("&", queryParameters);
        }

        //public class RemotePost
        //{
        //    private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        //    public string Url = "";
        //    public string Method = "post";
        //    public string FormName = "form1";
        //    public void Add(string name, string value)
        //    {
        //        Inputs.Add(name, value);
        //    }
        //    public void Post()
        //    {
        //        System.Web.HttpContext.Current.Response.Clear();
        //        System.Web.HttpContext.Current.Response.Write("<html><head>");
        //        System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
        //        System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
        //        for (int i = 0; i < Inputs.Keys.Count; i++)
        //        {
        //            System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
        //        }
        //        System.Web.HttpContext.Current.Response.Write("</form>");
        //        System.Web.HttpContext.Current.Response.Write("</body></html>");
        //        System.Web.HttpContext.Current.Response.End();
        //    }
        //}
        [HttpPost]
        public ActionResult PaymentSuccessful(string encResp)
        {
            var decryption = new CCACrypto();
            var decryptedParameters = decryption.Decrypt(encResp, ConfigurationManager.AppSettings["CcAvenueWorkingKey"].ToString());

            var keyValuePairs = decryptedParameters.Split('&');
            var splittedKeyValuePairs = new Dictionary<string, string>();

            foreach (var value in keyValuePairs)
            {
                var keyValuePair = value.Split('=');
                splittedKeyValuePairs.Add(keyValuePair[0], keyValuePair[1]);
            }

            if (splittedKeyValuePairs["order_status"] == "Success")
            {

                //Here you can check the consistency of data i.e what you send is what you get back,
                //Make sure its not corrupted....
                //After that Save the details of the transaction into a db if you want to...
                //I am just returning the data I got back...
                var loginUser = UserService.Instance.GetUserByUID(Convert.ToInt32(splittedKeyValuePairs["merchant_param2"]));
                // add order in order history

                using (var context = new SVIContext())
                {
                    Orders order = new Orders();
                    order.createdBy = Convert.ToInt32(splittedKeyValuePairs["merchant_param2"]);

                    order.ReceiverName = splittedKeyValuePairs["billing_name"];
                    order.ReceiverEmail = splittedKeyValuePairs["billing_email"];
                    order.ReceiverMobileNumber = splittedKeyValuePairs["delivery_tel"];
                    order.ShippingAddress = splittedKeyValuePairs["billing_address"];
                    order.City = splittedKeyValuePairs["delivery_city"];
                    order.PinCode = splittedKeyValuePairs["delivery_zip"];
                    order.TotalAmount = Convert.ToDecimal(splittedKeyValuePairs["amount"]);
                    order.DeliveryCharges = Convert.ToInt32(splittedKeyValuePairs["merchant_param1"]);
                    order.BillAmount = Convert.ToDecimal(splittedKeyValuePairs["amount"]) - Convert.ToInt32(splittedKeyValuePairs["merchant_param1"]);
                    order.rzp_orderid = splittedKeyValuePairs["tracking_id"];
                    order.currency = splittedKeyValuePairs["currency"];
                    order.razorpayKey = splittedKeyValuePairs["bank_ref_no"];

                    order.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    order.PaymentType = Convert.ToInt32(Constant.PaymentTypeEnum.Online);
                    order.PaymentStatus = Convert.ToInt32(Constant.PaymentStatusEnum.Complete);
                    order.OrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Pending);
                    var userCartData = CartService.Instance.GetListUserCart(loginUser.Email);
                    order.OrderItems = userCartData.Select(x => new OrderItems()
                    {
                        ItemId = x.ProductId,
                        Name = x.Product.Name,
                        Price = x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price,
                        Quantity = x.Quantity,
                        DDate = x.DDate,
                        SlotId = x.DSlot,
                        IDesc = x.Description,
                    }).ToList();

                    context.Orders.Add(order);
                    context.SaveChanges();
                    //Removed from Cart
                    var RemoveCart = new List<Cart>();
                    RemoveCart = context.Cart.Where(x => x.CreatedBy == loginUser.Email).ToList();
                    context.Cart.RemoveRange(RemoveCart);
                    context.SaveChanges();
                    var notification = new Notification();
                    notification.CreatedOn = DateTime.Now;
                    notification.isMultipleUser = false;
                    notification.Status = Constant.NotificationConstant.unread;
                    notification.AnchorLink = "/Admin/PendingOrdersList";
                    notification.Message = string.Format(Constant.NotificationConstant.AddNewOrder, order.OrderId.ToString());
                    var notificationAction = new NotificationactionController();
                    notificationAction.SaveNotification(notification);
                    MyHub objNotifHub = new MyHub();
                    objNotifHub.TotalOrderData(order.OrderId.ToString());
                    return RedirectToAction("Thankyou", new { OrderId = order.OrderId });
                }
            }
            else
            {
                return RedirectToAction("PaymentCancelled");
            }
        }
        #endregion



        [Route("YourOrders/{Email}/{Status}")]
        public ActionResult UserOrders(string Email, string Status)
        {
            try
            {
                var model = new UserOrdersDataModel();
                if (Email != null && Email != string.Empty)
                {
                    var loginUser = UserService.Instance.GetCurrentUserLogin();
                    if (loginUser != null && loginUser != "")
                    {
                        var user = UserService.Instance.GetUser(Email);
                        if (user != null)
                        {
                            model.CompleteOrderItems = OrdersService.Instance.GetUserComOrdersList(user.Id);
                            model.PendingOrderItems = OrdersService.Instance.GetUserPenOrderItemsList(user.Id);
                            model.Status = Status;
                        }
                        else
                        {
                            return RedirectToAction("Login", "Account");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account", new { url = "/YourOrders/" + Email + "/" + Status});
                    }
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult Rating(string PId, int starRating)
        {
            try
            {
                var LoginUser = UserService.Instance.GetCurrentUserLogin();
                if (LoginUser != null)
                {
                    int pId = Convert.ToInt32(PId);
                    HelperService.Instance.AddStarRating(pId, starRating);
                }
                return Json(starRating);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }

        [Route("Thankyou")]
        public ActionResult Thankyou(string OrderId, bool isEmailReceipt = false)
        {
            try
            {
                TempData["EmailReceipt"] = isEmailReceipt;
                var UserOrder = OrdersService.Instance.GetUserCurrentOrder(Convert.ToInt32(OrderId));
                return View(UserOrder);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        [HttpPost]
        public ActionResult CheckShipment(string City,string State,string Division,string Region,string pincode)
        {
            var result = "false";
            List<string> data = ((Constant.ShipmentCity[])Enum.GetValues(typeof(Constant.ShipmentCity))).Select(c => c.ToString()).ToList();
            if (data != null && State == "Maharashtra" && data.Contains(Region))
            {
                result = "true";
            }
            return Json(new { Result = result}, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string ShipmentCharge(string lat, string lon)
        {
            var result = "0";
            using (var context = new SVIContext())
            {
                var shipmentInfo = context.Keys.Where(x => x.Type == "ShipmentDistance" || x.Type == "ShipmentCo-ordinates" || x.Type == "ShipmentCharges");
                string Comlat = shipmentInfo.FirstOrDefault(x => x.Name == "lat").Description;
                string Comlon = shipmentInfo.FirstOrDefault(x => x.Name == "lon").Description;
                double maxdist = Convert.ToDouble(shipmentInfo.FirstOrDefault(x => x.Type == "ShipmentDistance").Description);
                var sCoord = new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lon));
                var eCoord = new GeoCoordinate(Convert.ToDouble(Comlat), Convert.ToDouble(Comlon));
                var aaa = sCoord.GetDistanceTo(eCoord);
                var aaakm = aaa/1000;
                if (aaakm > maxdist) 
                {
                    double extkm = aaakm - maxdist;
                    var shipcharges = shipmentInfo.Where(x => x.Type == "ShipmentCharges").ToList();
                    List<int> distanceCount = new List<int>();
                    shipcharges = shipcharges.Select(x =>
                    {
                        distanceCount.Add(Convert.ToInt32(x.Name));
                        return x;
                    }).ToList();
                    distanceCount = distanceCount.OrderBy(x=> x).ToList();
                    var close = distanceCount.Where(x => x > extkm).ToList() != null && distanceCount.Where(x => x > extkm).ToList().Count > 0 ? distanceCount.Where(x => x > extkm).First() : distanceCount.LastOrDefault();
                    result = shipcharges.FirstOrDefault(x => x.Name == close.ToString()).Description;
                }
                else
                {
                    result = "0";
                }

            }
            return result;
        }

        public ActionResult PaymentCancelled()
        {
            return View();
        }

        public class EnumModel
        {
            public string Name { get; set; }
        }

        //public ActionResult PaymentPage(Orders order)
        //{
        //    return View(order);
        //}

        //[HttpPost]
        //public ActionResult Complete(Orders order)
        //{
        //    try
        //    {
        //        // Payment data comes in url so we have to get it from url

        //        // This id is razorpay unique payment id which can be use to get the payment details from razorpay server
        //        string paymentId = Request.Params["rzp_paymentid"];

        //        // This is orderId
        //        string orderId = Request.Params["rzp_orderid"];


        //        Razorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(ConfigurationManager.AppSettings["KeyId"], ConfigurationManager.AppSettings["KeySecret"]);

        //        Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

        //        // This code is for capture the payment 
        //        Dictionary<string, object> options = new Dictionary<string, object>();
        //        options.Add("amount", payment.Attributes["amount"]);
        //        Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
        //        string amt = paymentCaptured.Attributes["amount"];

        //        //// Check payment made successfully

        //        if (paymentCaptured.Attributes["status"] == "captured")
        //        {
        //            //Emailget from current user
        //            var loginUser = UserService.Instance.GetCurrentUserLogin();
        //            // add order in order history
        //            order.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
        //            order.PaymentType = Convert.ToInt32(Constant.PaymentTypeEnum.Online);
        //            order.PaymentStatus = Convert.ToInt32(Constant.PaymentStatusEnum.Complete);
        //            order.OrderStatus = Convert.ToInt32(Constant.OrderStatusEnum.Pending);
        //            var userCartData = CartService.Instance.GetListUserCart(loginUser);
        //            order.OrderItems = userCartData.Select(x => new OrderItems()
        //            {
        //                ItemId = x.ProductId,
        //                Name = x.Product.Name,
        //                Price = x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price,
        //                Quantity = x.Quantity,
        //                DDate = x.DDate,
        //                SlotId = x.DSlot,
        //                IDesc = x.Description,
        //            }).ToList();
        //            using (var context = new SVIContext())
        //            {
        //                context.Orders.Add(order);
        //                context.SaveChanges();
        //                //Removed from Cart
        //                var RemoveCart = new List<Cart>();
        //                RemoveCart = context.Cart.Where(x => x.CreatedBy == loginUser).ToList();
        //                context.Cart.RemoveRange(RemoveCart);
        //                context.SaveChanges();
        //                var notification = new Notification();
        //                notification.CreatedOn = DateTime.Now;
        //                notification.isMultipleUser = false;
        //                notification.Status = Constant.NotificationConstant.unread;
        //                notification.AnchorLink = "/Admin/PendingOrdersList";
        //                notification.Message = string.Format(Constant.NotificationConstant.AddNewOrder, order.OrderId.ToString());
        //                var notificationAction = new NotificationactionController();
        //                notificationAction.SaveNotification(notification);
        //                MyHub objNotifHub = new MyHub();
        //                objNotifHub.TotalOrderData(order.OrderId.ToString());
        //                return RedirectToAction("Thankyou", new { OrderId = order.OrderId });
        //            }
        //            // return to success page with userid
        //        }
        //        else
        //        {
        //            return RedirectToAction("Failed");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return Redirect("~/Not_found.html");
        //    }
        //}

        //public ActionResult Failed()
        //{
        //    return View();
        //}
    }
}