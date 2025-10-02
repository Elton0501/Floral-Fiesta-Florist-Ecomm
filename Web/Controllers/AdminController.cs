using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using Database.Migrations;
using DataModels;
using Entities;
using Microsoft.Ajax.Utilities;
using Services;
using SV.Database;

namespace Web.Controllers
{
    [AdminAuthorizationFilterAttribute]
    public class AdminController : Controller
    {
        private string cs;
        public AdminController()
        {
            cs = ConfigurationManager.ConnectionStrings["SVIDB"].ConnectionString;
        }

        // GET: Admin
        #region Dashboard
        public ActionResult Index()
        {
            try
            {
                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                var model = new AdminViewModel();
                var orders = OrdersService.Instance.GetAllOrders();
                var users = UserService.Instance.GetUsers();
                var Products = ProductService.Instance.GetAllProducts();
                var OrderItems = OrdersService.Instance.GetAllOrderItems();
                model.TodayOrdersCount = orders.Where(x => x.CreatedOn.Date == today.Date).ToList().Count();
                model.TodayEarning = orders.Where(x => x.CreatedOn.Date == today.Date).Select(x => x.TotalAmount).Sum();
                model.MonthlyOrdersCount = orders.Where(x => x.CreatedOn.Month == today.Month).ToList().Count();
                model.MonthlyEarning = orders.Where(x => x.CreatedOn.Month == today.Month).Select(x => x.TotalAmount).Sum();
                model.YearlyOrdersCount = orders.Where(x => x.CreatedOn.Year == today.Year).ToList().Count();
                model.YearlyEarning = orders.Where(x => x.CreatedOn.Year == today.Year).Select(x => x.TotalAmount).Sum();
                model.TotalUser = users.Count();
                model.TotalProduct = ProductService.Instance.GetAllProducts().Count();
                orders = orders.Select(x =>
                {
                    x.User = users.FirstOrDefault(z => z.Id == x.createdBy);
                    return x;
                });
                OrderItems = OrderItems.Select(x =>
                {
                    x.Product = Products.FirstOrDefault(z => z.Id == x.ItemId);
                    return x;
                });
                model.userOrdersCounts = orders.Select(x => new UserOrdersCount()
                {
                    City = x.City,
                    Contact = x.User.MobileNumber,
                    Email = x.User.Email,
                    Name = x.User.Name,
                    UId = x.User.Id,
                    Count = orders.Where(q => q.createdBy == x.createdBy).ToList().Count(),
                }).DistinctBy(w => w.UId).ToList();
                model.OrderItemsCount = OrderItems.Select(x => new OrderItemsCount()
                {
                    Name = x.Product.Name,
                    PId = x.Product.Id,
                    Count = OrderItems.Where(q => q.ItemId == x.ItemId).ToList().Count(),
                }).DistinctBy(w => w.PId).ToList();
                return View(model);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        #endregion
        #region Key
        public ActionResult WebsiteInformation()
        {
            using (var context = new SVIContext())
            {
                var keys = context.Keys.ToList();
                return View(keys);
            }
        }
        public ActionResult InfoAdd()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult InfoAdd(Key Key)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    Key.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    context.Keys.Add(Key);
                    context.SaveChanges();
                    result = "true";
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ActionResult InfoEdit(int id) 
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var key = context.Keys.FirstOrDefault(x => x.Id == id);
                    return PartialView(key);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult InfoEdit(Key key)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    context.Entry(key).State = EntityState.Modified;
                    context.SaveChanges();
                    result = "true";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(result);
        }

        public bool InfoDelete(int id)
        {
            bool result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var delete = context.Keys.Find(id);
                    context.Keys.Remove(delete);
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
        #endregion
        #region Category
        public ActionResult Categories()
        {
            using (var context = new SVIContext())
            {
                var categories = context.Categories.ToList();
                return View(categories);
            }
        }
        [HttpPost]
        public ActionResult CategoryStatus(int catId, bool status)
        {
            var result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var categories = context.Categories.FirstOrDefault(x=>x.Id == catId);
                    categories.Status = status;
                    context.Entry(categories).State = EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(result);
        }
        public ActionResult CategoryAdd()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CategoryAdd(Category category)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    category.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    category.Status = true;
                    context.Categories.Add(category);
                    context.SaveChanges();
                    result = "true";
                }
            }
            catch (Exception ex)
            {
                return View(category);
                throw ex;
            }
            return Json(result);
        }
        public ActionResult CategoryEdit(int id)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var category = context.Categories.FirstOrDefault(x => x.Id == id);
                    return PartialView(category);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult CategoryEdit(Category category)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    context.Entry(category).State = EntityState.Modified;
                    context.SaveChanges();                    
                }
                ImageController imgc = new ImageController();
                if (category.BannerImage != category.oldbimg)
                {
                    imgc.DeleteImg(category.oldbimg);
                }
                if (category.CoverImage != category.oldcimg)
                {
                    imgc.DeleteImg(category.oldcimg);
                }
                result = "true";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);

        }
        #endregion
        #region Subcategories
        public ActionResult SubCategories()
        {
            using (var context = new SVIContext())
            {
                var subcategories = context.SubCategories.Include(x => x.Category).ToList();
                return View(subcategories);
            }
        }
        public ActionResult SubCategoryStatus(int scatId, bool status)
        {
            var result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var subcategories = context.SubCategories.FirstOrDefault(x => x.Id == scatId);
                    subcategories.Status = status;
                    context.Entry(subcategories).State = EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(result);
        }
        public ActionResult SubCategoryAdd()
        {
            using (var context = new SVIContext())
            {
                var catList = new List<SelectListItem>();
                var category = context.Categories.ToList();
                for (int i = 0; i < category.Count; i++)
                {
                    var catData = new SelectListItem() { Text = category[i].Name, Value = category[i].Id.ToString() };
                    catList.Add(catData);
                }
                ViewBag.CatItem = catList;
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult SubCategoryAdd(SubCategory subcategory)
        {
            string result;
            try
            {               
                using (var context = new SVIContext())
                {
                    subcategory.Create = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    subcategory.Status = true;
                    context.SubCategories.Add(subcategory);
                    context.SaveChanges();
                    result = "true";
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult SubCategoryEdit(int id)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var catList = new List<SelectListItem>();
                    var category = context.Categories.ToList();
                    for (int i = 0; i < category.Count; i++)
                    {
                        var catData = new SelectListItem() { Text = category[i].Name, Value = category[i].Id.ToString() };
                        catList.Add(catData);
                    }
                    ViewBag.CatItem = catList;
                    var subcategory = context.SubCategories.FirstOrDefault(x => x.Id == id);
                    return PartialView(subcategory);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult SubCategoryEdit(SubCategory subcategory)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    context.Entry(subcategory).State= EntityState.Modified;
                    context.SaveChanges();
                    result= "true";
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetSubCategories(int CatId)
        {
            using (var context = new SVIContext())
            {
                var subCat =new List<SubCatDropDown>();
                var subcategories = context.SubCategories.Where(x=>x.Status == true && x.CategoryId == CatId).ToList();
                subCat = subcategories.Select(x => new SubCatDropDown
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList();
                var serializer = new JavaScriptSerializer();
                var data = serializer.Serialize(subCat);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Product
        public ActionResult Product()
        {
            return View();
            //using (var context = new SVIContext())
            //{
            //    var product = context.Products.Include(x => x.ProductImages).Include(x=>x.SubCategory).OrderByDescending(x => x.Id).ToList();
            //    return View(product);
            //}
        }
        [HttpPost]
        public ActionResult ProductList(int draw, int start, int length, int dropdownId)
        {
            var data = new List<Product>();
            int totalFilterdRecord = 0;
            int totalRecord = 0;

            try
            {
                string sortColumnIndex = Request.Form.GetValues("order[0][column]")?.FirstOrDefault();
                string sortColumnDirection = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault()?.ToUpper();

                sortColumnIndex = sortColumnIndex != null ? sortColumnIndex : "0";
                sortColumnDirection = sortColumnDirection != null ? (sortColumnDirection == "ASC" ? "ASC" : "DESC") : "DESC";

                string searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();

                using (var conn = new SqlConnection(cs))
                {
                    using (var cmd = new SqlCommand("jeetn.FilterProducts", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Start", start);
                        cmd.Parameters.AddWithValue("@Length", length);
                        cmd.Parameters.AddWithValue("@SearchValue", !string.IsNullOrEmpty(searchValue) ? searchValue.Trim() : "");
                        cmd.Parameters.AddWithValue("@SortColumnIndex", Convert.ToInt32(sortColumnIndex));
                        cmd.Parameters.AddWithValue("@SortDirection", sortColumnDirection);
                        cmd.Parameters.AddWithValue("@StatusType", dropdownId);
                        conn.Open();
                        using (var sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                data.Add(new Product
                                {
                                    Id = Convert.ToInt32(sdr["Id"]),
                                    Name = sdr["Name"].ToString(),
                                    DefaultImage = sdr["Image"] != DBNull.Value ? sdr["Image"].ToString() : null,
                                    SubCategoryName = sdr["SubCategoryName"] != DBNull.Value ? sdr["SubCategoryName"].ToString() : null,
                                    CreatedOn = Convert.ToDateTime(sdr["CreatedOn"]),
                                    Isfeatured = Convert.ToBoolean(sdr["Isfeatured"]),
                                    isWhatsapp = Convert.ToBoolean(sdr["isWhatsapp"]),
                                    Status = Convert.ToBoolean(sdr["Status"]),
                                    SeoTags = sdr["SeoTags"] != DBNull.Value ? sdr["SeoTags"].ToString() : null,
                                    Rating = sdr["Rating"] != DBNull.Value ? (int?)Convert.ToInt32(sdr["Rating"]) : null,
                                    CreatedOnString = Convert.ToDateTime(sdr["CreatedOn"]).ToString("dd MMM yyyy"),
                                });
                            }

                            if (sdr.NextResult())
                            {
                                if (sdr.Read())
                                {
                                    totalFilterdRecord = sdr.IsDBNull(0) ? 0 : sdr.GetInt32(0);
                                }
                            }
                            
                            if (sdr.NextResult())
                            {
                                if (sdr.Read())
                                {
                                    totalRecord = sdr.IsDBNull(0) ? 0 : sdr.GetInt32(0);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            var result = new
            {
                draw,
                recordsTotal = totalRecord,
                recordsFiltered = totalFilterdRecord,
                data = data
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductStatus(int proId, bool status)
        {
            var result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var product = context.Products.FirstOrDefault(x => x.Id == proId);
                    product.Status = product.Status ? false: true;
                    context.Entry(product).State = EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(result);
        }
        public ActionResult ProductAdd()
        {
            using (var context = new SVIContext())
            {
                var catList = new List<SelectListItem>();
                var category = context.Categories.ToList();
                for (int i = 0; i < category.Count; i++)
                {
                    var catData = new SelectListItem() { Text = category[i].Name, Value = category[i].Id.ToString() };
                    catList.Add(catData);
                }
                ViewBag.CatItem = catList;
            }
            return PartialView();
        }
        [HttpPost]
        public ActionResult ProductAdd(Product product)
        {
            string result;
            try
            {
 
                using (var context = new SVIContext())
                {
                    product.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    product.Status = true;
                    string[] PimgList = product.PImageName.Split(',');
                    product.ProductImages = PimgList.Select(x => new ProductImage()
                    {
                        Image = x,
                        Default = x.Trim() == product.imgdefault ? true : false,
                        Create = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"))
                    }).ToList();
                    context.Products.Add(product);
                    context.SaveChanges();
                    result = "true";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);
        }
        public ActionResult ProductEdit(int id)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    string imgdata = "";
                    var product = context.Products.Include(x=>x.ProductImages).FirstOrDefault(x => x.Id == id);
                    for (int i = 0; i < product.ProductImages.Count(); i++)
                    {                        
                        if (i < (product.ProductImages.Count() - 1))
                        {                           
                            imgdata = imgdata + product.ProductImages[i].Image + ",";
                        }
                        else
                        {
                            imgdata = imgdata + product.ProductImages[i].Image;
                        }
                    }
                    var catList = new List<SelectListItem>();
                    var category = context.Categories.ToList();
                    for (int i = 0; i < category.Count; i++)
                    {
                        var catData = new SelectListItem() { Text = category[i].Name, Value = category[i].Id.ToString() };
                        catList.Add(catData);
                    }

                    var subCatList = new List<SelectListItem>();
                    var subCategory = context.SubCategories.Where(x=>x.CategoryId == product.CategoryId).ToList();
                    for (int i = 0; i < subCategory.Count; i++)
                    {
                        var subcatData = new SelectListItem() { Text = subCategory[i].Name, Value = subCategory[i].Id.ToString() };
                        subCatList.Add(subcatData);
                    }

                    var defaultimg = context.ProductImages.FirstOrDefault(x => x.ProductId == id && x.Default == true);
                    ViewBag.DefaultImage = defaultimg != null ? defaultimg.Image : null;
                    ViewBag.CatItem = catList;
                    ViewBag.SubCatItem = subCatList;
                    ViewBag.PimgString = imgdata;
                    return PartialView(product);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult ProductEdit(Product product)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    var productImage = context.ProductImages.Where(x => x.ProductId == product.Id).ToList();
                    context.ProductImages.RemoveRange(productImage);
                    context.SaveChanges();
                    string[] PimgList = null;
                    string imgdelstring = product.OldPImageName;
                    if (product.PImageName != null && product.PImageName.Length > 0)
                    {
                        PimgList = product.PImageName.Split(',');
                        var imgdelList = new List<ImageModel>();
                        foreach (var img in PimgList)
                        {
                            imgdelstring = imgdelstring.Replace(img, "");
                        }
                    }
                    if (imgdelstring != null)
                    {
                        while (imgdelstring.Contains(",,"))
                        {
                            imgdelstring = imgdelstring.Replace(",,", ",");
                        }
                        if (imgdelstring.StartsWith(","))
                        {
                            imgdelstring = imgdelstring.Remove(0, 1);
                        }
                        if (imgdelstring.EndsWith(","))
                        {
                            imgdelstring = imgdelstring.Remove(imgdelstring.Length - 1, 1);
                        }
                        string[] imgdelarr = imgdelstring.Split(',');
                        foreach (var dimg in imgdelarr)
                        {
                            ImageController Image = new ImageController();
                            Image.DeleteImg(dimg);
                        }
                    }
                    if (product.PImageName != null && product.PImageName.Length > 0)
                    {
                        var plistImg = new List<ProductImage>();
                        foreach (var pimg in PimgList)
                        {
                            var singleproductImg = new ProductImage();
                            singleproductImg.Image = pimg;
                            singleproductImg.ProductId = product.Id;
                            singleproductImg.Default = pimg == product.imgdefault ? true : false;
                            singleproductImg.Create = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                            plistImg.Add(singleproductImg);
                        }
                        context.ProductImages.AddRange(plistImg);
                    }
                    context.Entry(product).State = EntityState.Modified;
                    context.SaveChanges();
                    result = "true";
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
        #region PendingOrders
        public ActionResult PendingOrdersList()
        {
            try
            {
                OrdersViewModel Orders = new OrdersViewModel();
                Orders.OrderStatusEnum = Enum.GetValues(typeof(Constant.OrderStatusEnum))
                .Cast<Constant.OrderStatusEnum>()
                .Select(d => new OrderStatusViewModel()
                {
                    Id = (int)d,
                    Name = d.ToString(),
                }).ToList();
                Orders.PaymentStatusEnum = Enum.GetValues(typeof(Constant.PaymentStatusEnum))
                .Cast<Constant.PaymentStatusEnum>()
                .Select(d => new PaymentStatusViewModel()
                {
                    Id = (int)d,
                    Name = d.ToString(),
                }).ToList();
                // Orders.Orders = OrdersService.Instance.GetPendingOrders();
                return View(Orders);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        public ActionResult _PendingOrdersList(string[] OrderStatus ,int? PaymentStatus,DateTime? StartDate, DateTime? EndDate)
        {
            OrdersViewModel Orders = new OrdersViewModel();
            Orders.OrderStatusEnum = Enum.GetValues(typeof(Constant.OrderStatusEnum))
            .Cast<Constant.OrderStatusEnum>()
            .Select(d => new OrderStatusViewModel()
            {
                Id = (int)d,
                Name = d.ToString(),
            }).ToList();
            Orders.PaymentStatusEnum = Enum.GetValues(typeof(Constant.PaymentStatusEnum))
            .Cast<Constant.PaymentStatusEnum>()
            .Select(d => new PaymentStatusViewModel()
            {
                Id = (int)d,
                Name = d.ToString(),
            }).ToList();
            Orders.Orders = OrdersService.Instance.GetPendingOrders(OrderStatus,PaymentStatus, StartDate, EndDate);
            return PartialView(Orders);
        }
        public ActionResult OrderDetails(int OrderID)
        {
            OrderDetailsViewModel model = new OrderDetailsViewModel();
            model.Order = OrdersService.Instance.GetOrderDetails(OrderID);
            model.OrderItems = OrdersService.Instance.GetOrderItems(OrderID);
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult OrderStatusChanged(int OrderID, int Status)
        {
            try
            {
                var result = "false";
                var data = OrdersService.Instance.GetOrderDetails(OrderID);
                result = OrdersService.Instance.OrderStatusChanged(OrderID, Status);
                if (result == "true" && Status == Convert.ToInt32(Constant.OrderStatusEnum.Complete))
                {
                    HelperController helperController = new HelperController();
                    string link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/YourOrders/" + data.ReceiverEmail + "/previous";
                    string invlink = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/Thankyou?OrderId=" + OrderID + "&isEmailReceipt=true";
                    string subject = "Delivered: Your Floral Fiesta order has been delivered.";
                    string Head = "Your Floral Fiesta order has been delivered.";
                    string message = "<p>Order no: " + OrderID.ToString() + "</p></br>" + "Hey " + data.User.Name + "</br>"
                        + "<p>We understand that you've received your order on time. \r\n If you</p></br>"
                        + "<p><a style=\"text-decoration:none; color:#417F34;\" href=\"" + invlink + "\">Click here</a> to view your order invoice</p>"
                        + "<p>Don't forget to rate our services and recommend us to your family and friends. Thank you for trusting us and making us a part of your celebrations. We hope you had a memorable gifting experience.</p></br>"
                        + "<p>To rate our products </p><a href=\"" + link + "\" style=\"border:1px solid #417F34; color:#417F34; padding: 5px 10px; text-decoration:none;\">Click Here</a>";
                    helperController.templateEmail(data.ReceiverEmail, subject, Head, message);
                }
                else if (result == "true" && Status == Convert.ToInt32(Constant.OrderStatusEnum.Processing))
                {
                    HelperController helperController = new HelperController();
                    string link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/YourOrders/"+ data.ReceiverEmail;
                    string subject = "Floral Fiesta: Order Confirm";
                    string Head = "Your Floral Fiesta order has been received. \r\n";
                    string message = "<p>Order no: " + OrderID.ToString() + "</p></br>" + "Hey " + data.User.Name + ",</br>"
                        + "<p>We know you're super excited to gift this bouquet to your special someone so we understand how important its timely delivery is.</p></br>"
                        + "<p>Your order has been received and is currently being curated with extreme care and love. Once, it is ready you'll get a mail confirmation.</p></br>"
                        + "<p>Thank you for choosing Floral Fiesta as your gifting partner, we appreciate it. For any queries, please be free to reach out.</p></br>";
                    helperController.templateEmail(data.ReceiverEmail, subject, Head, message);
                }
                else if (result == "true" && Status == Convert.ToInt32(Constant.OrderStatusEnum.Ready))
                {
                    HelperController helperController = new HelperController();
                    string link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/YourOrders/" + data.ReceiverEmail + "/current";
                    string subject = "Ready: Your Floral Fiesta order is ready for shipping.";
                    string Head = "Your Floral Fiesta order is ready for shipping.";
                    string message = "<p>Good News!</p></br>"
                        + "<p>Hey </p>" + data.User.Name + "</br>"
                        + "<p>We've got some exciting news. Your order is ready for shipping. Once it ships, you'll receive a mail confirmation.</p></br>"
                       + "<p>To check ur order status </p><a href=\"" + link + "\" style=\"border:1px solid #417F34; color:#417F34; text-decoration:none; padding: 5px 10px;\">Click Here</a>";
                    helperController.templateEmail(data.ReceiverEmail, subject, Head, message);
                }
                else if (result == "true" && Status == Convert.ToInt32(Constant.OrderStatusEnum.Dispatched))
                {
                    HelperController helperController = new HelperController();
                    string link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "#BestSeller";
                    string subject = "Shipped: Your Floral Fiesta Order has been dispatched for delivery";
                    string Head = "Your Floral Fiesta Order has been dispatched for delivery.";
                    string message = "<p>Your Order Has Shipped!</br>"
                        + "<p>Hey </p>" + data.User.Name + "</br>"
                        + "<p>We know you can't wait for your package to arrive.</p>" +
                        "While we make sure your order reaches you on time, you can browse our <a style=\"text-decoration:none; color:#417F34;\" href=\"" + link+"\">Bestsellers and New Arrivals</a> for your next special present. For any queries regarding the status of your order, please be free to reach out.";
                    helperController.templateEmail(data.ReceiverEmail, subject, Head, message);
                }

                return Json(new {response = result });
            }
            catch (Exception ex)
            {
                return Redirect("~/Not_found.html");
            }
        }
        [HttpPost]
        public ActionResult OrderPaymentChanged(int OrderID, int Status)
        {
            try
            {
                var result = "false";
                result = OrdersService.Instance.OrderPaymentChanged(OrderID, Status);
                return Json(new {response = result });
            }
            catch (Exception ex)
            {
                return Redirect("~/Not_found.html");
            }
        }
        #endregion
        #region CompleteOrders
        public ActionResult CompleteOrdersList()
        {
            OrdersViewModel Orders = new OrdersViewModel();
            Orders.OrderStatusEnum = Enum.GetValues(typeof(Constant.OrderStatusEnum))
            .Cast<Constant.OrderStatusEnum>()
            .Select(d => new OrderStatusViewModel()
            {
                Id = (int)d,
                Name = d.ToString(),
            }).ToList();
            Orders.PaymentStatusEnum = Enum.GetValues(typeof(Constant.PaymentStatusEnum))
            .Cast<Constant.PaymentStatusEnum>()
            .Select(d => new PaymentStatusViewModel()
            {
                Id = (int)d,
                Name = d.ToString(),
            }).ToList();
            //  Orders.Orders = OrdersService.Instance.GetCompleteOrdersList();
            return View(Orders);
        }
        public ActionResult _CompleteOrdersList(DateTime? StartDate, DateTime? EndDate)
        {
            //var data = OrdersService.Instance.GetCompleteOrdersList();
            //return View(data);
            OrdersViewModel Orders = new OrdersViewModel();
            Orders.OrderStatusEnum = Enum.GetValues(typeof(Constant.OrderStatusEnum))
            .Cast<Constant.OrderStatusEnum>()
            .Select(d => new OrderStatusViewModel()
            {
                Id = (int)d,
                Name = d.ToString(),
            }).ToList();
            Orders.Orders = OrdersService.Instance.GetCompleteOrdersList(StartDate, EndDate);
            return View(Orders);
        }
        #endregion
        #region Charts
        public ActionResult pieChart()
        {
            var result = new List<WebCharts>();
            var PaymentOrder = new List<PaymentMethodOrders>();
            try
            {
                using (var context = new SVIContext())
                {
                    var orders = OrdersService.Instance.GetAllOrders();
                    PaymentOrder = orders.Select(x => new PaymentMethodOrders()
                    {
                        PaymentMethodInt = x.PaymentType,
                        PaymentMethod = Convert.ToInt32(Constant.PaymentTypeEnum.COD)== x.PaymentType ? "COD" : "Online Payment",
                        Count = orders.Where(z => z.PaymentType == x.PaymentType).ToList().Count()
                    }).ToList();
                    result = PaymentOrder.Select(x => new WebCharts()
                    {
                        Key = x.PaymentMethod,
                        Value = x.Count
                    }).DistinctBy(x => x.Key).OrderByDescending(x => x.Value).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult doughnut()
        {
            var result = new List<WebCharts>();
            var Cityresult = new List<CityOrders>();
            try
            {
                using (var context = new SVIContext())
                {
                    var orders = OrdersService.Instance.GetAllOrders();
                    Cityresult = orders.Select(x => new CityOrders()
                    {
                        City = x.City,
                        Count = orders.Where(z=>z.City == x.City).ToList().Count()
                    }).ToList();
                    result = Cityresult.Select(x => new WebCharts()
                    {
                        Key = x.City,
                        Value = x.Count
                    }).DistinctBy(x => x.Key).OrderByDescending(x => x.Value).ToList();
                    result = result.Count() > 8 ? result.Take(8).ToList() : result;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult barChart(string First, string Second)
        {
            DateTime first = First != null && First != string.Empty ? Convert.ToDateTime(First) : TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")).AddDays(-7);
            DateTime second = Second != null && Second != string.Empty ? Convert.ToDateTime(Second).AddDays(1) : TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            var result = new List<WebCharts>();
            try
            {
                using (var context = new SVIContext())
                {
                    var orders = OrdersService.Instance.GetAllOrders().Where(x => x.CreatedOn < second && x.CreatedOn >= first);
                    result = orders.Select(x => new WebCharts()
                    {
                        Key = x.CreatedOn.Date.ToString("MMM-dd"),
                        Value = orders.Where(y => y.CreatedOn.Date == x.CreatedOn.Date).Count()
                    }).DistinctBy(x => x.Key).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region User
        public ActionResult UserList()
        {
            try
            {
                var data = UserService.Instance.GetUsers();
                return View(data);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        public ActionResult DeleteUser(int Id)
        {
            try
            {
                UserService.Instance.RemoveUser(Id);
                return RedirectToAction("UserList");
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        public ActionResult UserAddress(int Id)
        {
            using(var context = new SVIContext())
            {
                var data = context.Addresses.Where(x => x.UserId == Id).ToList();
                return PartialView(data);
            }
        }
        #endregion
        #region Newsletter
        public ActionResult Newsletter()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Newsletter(string Sub, string Head, string Body)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    var testmail = context.Keys.Where(x => x.Name == "testemail").ToList();
                    if (testmail.Count > 0)
                    {
                        foreach (var email in testmail)
                        {
                            newslettermail(Sub, Head, Body.ToString(), email.Description);
                        }
                    }
                    result = "true";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result);
        }
        public ActionResult NewsletterFinal(string Sub, string Head, string Body)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    var maillist = context.Newsletter.ToList();
                    if (maillist.Count > 0)
                    {
                        foreach (var email in maillist)
                        {
                            newslettermail(Sub, Head, Body.ToString(), email.Email);
                        }
                    }
                    result = "true";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Json(result);
        }
        public void newslettermail(string Sub, string Head, string Body, string email)
        {
            try
            {

                var mail = ConfigurationManager.AppSettings["email"];
                var pass = ConfigurationManager.AppSettings["pass"];
                var port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                var stp = ConfigurationManager.AppSettings["smtp"];
                var ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["ssl"]);

                string FilePath = HostingEnvironment.MapPath("~/EmailTemplate/") + "NewsLTemp" + ".cshtml";
                StreamReader reader = new StreamReader(FilePath);
                string textMail = reader.ReadToEnd();
                reader.Close();

                textMail = textMail.Replace("[head]", Head);
                textMail = textMail.Replace("[content]", Body);


                MailMessage message = new MailMessage();
                message.From = new MailAddress(mail);
                message.To.Add(email);
                message.Subject = Sub;
                message.Body = textMail;

                message.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(stp, port);
                smtp.Credentials = new NetworkCredential(mail, pass);
                smtp.EnableSsl = ssl;
                smtp.Send(message);
                smtp.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Seasonal
        public ActionResult Seasonal()
        {
            using (var context = new SVIContext())
            {
                var products = context.Products.Include(x=>x.SubCategory).OrderBy(x => x.SubCategory.Name).ToList();
                return View(products);
            }           
        }
        [HttpPost]
        public ActionResult SeasonStatus(int proId, bool status)
        {
            var result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var product = context.Products.FirstOrDefault(x => x.Id == proId);
                    product.Seasonal = status;
                    context.Entry(product).State = EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult FeatureStatus(int proId, bool status)
        {
            var result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var product = context.Products.FirstOrDefault(x => x.Id == proId);
                    product.Isfeatured = status;
                    context.Entry(product).State = EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult SeasonSt(int staId, string status)
        {
            var result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var key = context.Keys.FirstOrDefault(x => x.Id == staId);
                    key.Name = status;
                    context.Entry(key).State = EntityState.Modified;
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Json(result);
        }
        public ActionResult SeasonImage()
        {
            try
            {
                return PartialView();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Testimonials
        public ActionResult Testimonials()
        {
            using (var context = new SVIContext())
            {
                var tes = context.Testemonials.OrderBy(x=>x.Id).ToList();
                return View(tes);
            }
        }
        public ActionResult TestimonialsAdd()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult TestimonialsAdd(Testemonial testemonial)
        {
            string result;
            try
            {
                using (var context = new SVIContext())
                {
                    testemonial.Created = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                    context.Testemonials.Add(testemonial);
                    context.SaveChanges();
                    result = "true";
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult TestemonialEdit(int id)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var tes = context.Testemonials.FirstOrDefault(x => x.Id == id);
                    return PartialView(tes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public ActionResult TestemonialEdit(int id, string name,string testimonialString)
        {
            string result;
            try
            {
                var testemonial = new Testemonial();
                testemonial.Id = id;
                testemonial.Created = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                testemonial.Testimonial = testimonialString;
                testemonial.Name = name;
                using (var context = new SVIContext())
                {
                    context.Entry(testemonial).State = EntityState.Modified;
                    context.SaveChanges();
                    result = "true";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(result);
        }
        public bool TestemonialsDelete(int id)
        {
            bool result = false;
            try
            {
                using (var context = new SVIContext())
                {
                    var delete = context.Testemonials.Find(id);
                    context.Testemonials.Remove(delete);
                    context.SaveChanges();
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            return result;
        }
        #endregion
    }
}