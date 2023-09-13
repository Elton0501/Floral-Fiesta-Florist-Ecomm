using Database.Migrations;
using DataModels;
using Entities;
using Microsoft.Ajax.Utilities;
using Services;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Web.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        [Route("Cart")]
        public ActionResult Index()
        {
            var data = new CartView();
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser == null || loginUser == string.Empty) { return RedirectToAction("Login", "Account"); };
                data.Carts = CartService.Instance.GetListUserCart(loginUser);
                data.TotalAmount = data.Carts.Count() > 0 ? data.Carts.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum() : 0;
                data.TotalwithoutDisc = data.Carts.Count() > 0 ? data.Carts.Select(x => x.Product.Price * x.Quantity).Sum() : 0;
                return View(data);

            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult AddInCart(int ProductId, int Quantity = 1)
        {
            var model = new CartDataModel();
            model.result = false;
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser == null || loginUser == string.Empty) {
                    model.isLogin = "false";
                    model.result = false;
                }
                else
                {
                    model = CartService.Instance.AddInCart(ProductId, loginUser, Quantity);
                };
            }
            catch (Exception)
            {
                model.result = false;
            }
            return Json(new { Result = model.result, Count = model.Count, TotalAmount = model.TotalAmount, isLogin = model.isLogin, CartId=model.CartId });
        }
        [HttpPost]
        public JsonResult RemoveInCart(int CartItemId, bool homePage)
        {
            var model = new CartDataModel();
            model.result = false;
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                model.TotalAmount = 0;
                model.TotalwithoutDisc = 0;
                if (loginUser == null) { model.result = false; }
                else
                {
                    model = CartService.Instance.RemoveInCart(CartItemId, loginUser, homePage);
                }
            }
            catch (Exception)
            {
                model.result = false;
            }
            return Json(new { Result = model.result, Count = model.Count, TotalAmount = model.TotalAmount, TotalwithoutDisc = model.TotalwithoutDisc });
        }
        [HttpPost]
        public ActionResult CartQuantity(int CartItemId, int CartQuantity)
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var loginUser = UserService.Instance.GetCurrentUserLogin();
                    if (loginUser == "" || loginUser == null) { return RedirectToAction("Login", "Account"); };
                    var ItemPrice = "0";
                    var ItemDisPrice = "0";
                    var cartItem = context.Cart.FirstOrDefault(x => x.Id == CartItemId);
                    var products = context.Products.ToList();
                    cartItem.Product = products.FirstOrDefault(x => x.Id == cartItem.ProductId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity = CartQuantity >= 1 ? CartQuantity : 1;
                        context.Entry(cartItem).State = EntityState.Modified;
                        context.SaveChanges();
                        ItemPrice = Convert.ToString(cartItem.Quantity * (cartItem.Product.DiscountPrice > 0 ? cartItem.Product.DiscountPrice : cartItem.Product.Price));
                        ItemDisPrice = Convert.ToString(cartItem.Quantity * cartItem.Product.Price);
                    }
                    var cartdata = context.Cart.Where(x => x.CreatedBy == loginUser).ToList();
                    cartdata = cartdata.Select(x =>
                    {
                        x.Product = products.FirstOrDefault(y => y.Id == x.ProductId);
                        return x;
                    }).ToList();
                    var TotalAmount = cartdata.Count() > 0 ? cartdata.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum() : 0;
                    var TotalWithoutDiscount = cartdata.Count() > 0 ? cartdata.Select(x => x.Product.Price * x.Quantity).Sum() : 0;
                    var cartQuant = cartdata.Select(x => x.Quantity).Sum();
                    return Json(new
                    {
                        TotalAmount = TotalAmount.ToString(),
                        cartQuant = cartQuant,
                        totalWithoutDiscount = TotalWithoutDiscount.ToString(),
                        itemPrice = ItemPrice,
                        itemDisPrice = ItemDisPrice
                    });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }
        [HttpPost]
        public JsonResult getSelectSlot(int Id, DateTime date)
        {
            var result = new selectSlot();
            result.result = false;
            using (var context = new SVIContext())
            {
                var cartitem = context.Cart.FirstOrDefault(x => x.Id == Id);
                int PId = cartitem.ProductId;
                var product = context.Products.FirstOrDefault(x => x.Id == PId);
                var cat = context.Categories.FirstOrDefault(x => x.Id == product.CategoryId);
                List<Key> todaySlots = context.Keys.Where(x => x.Type == "Slot").ToList();
                DateTime todayDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                if (cat.Name == "Bouquets")
                {
                          
                    if (date == todayDate.Date && todayDate.Hour < 17)
                    {
                        result.slot = CartService.Instance.SlotNumber(todayDate.Hour);
                        result.date = DateTime.Now.Date.ToString();
                        result.PId = PId.ToString();
                        result.result = true;
                    }

                    if ( todayDate.Hour > 19 && todayDate.AddDays(1).Date == date)
                    {
                        var selectedSlots = todaySlots.Where(x => x.Name != "1").ToList();
                        result.date = date.ToString();
                        result.slot = selectedSlots;
                        result.PId = PId.ToString();
                        result.result = true;
                    }

                    else if (date > todayDate)
                    {
                        result.date = date.ToString();
                        result.slot = todaySlots;
                        result.PId = PId.ToString();
                        result.result = true;
                    }

                }
                else
                {
                    if (todayDate.Date == date.Date)
                    {
                        result.date = date.ToString();
                        result.PId = PId.ToString();
                        result.result = false;
                    }
                    else if(todayDate.Date < date.Date)
                    {
                        var selectedSlots = todaySlots.ToList();
                        result.date = date.ToString();
                        result.slot = selectedSlots;
                        result.PId = PId.ToString();
                        result.result = true;
                    }
                    if (todayDate.Hour >= 15 && todayDate.Date.AddDays(1) == date.Date)
                    {
                        var selectedSlots = todaySlots.Where(x => x.Name != "1" && x.Name != "2").ToList();
                        result.date = date.ToString();
                        result.slot = selectedSlots;
                        result.PId = PId.ToString();
                        result.result = true;
                    }
                }
                var serializer = new JavaScriptSerializer();
                var data = serializer.Serialize(result);
                return Json(data);
            }
        }

        public string SaveSlot(int cartId, string date, string Slot)
        {
            var result = "Something went wrong";
            using (var context = new SVIContext())
            {
                var cartitem = context.Cart.FirstOrDefault(x => x.Id == cartId);
                cartitem.DSlot = Slot;
                cartitem.DDate = date;
                context.Entry(cartitem).State = EntityState.Modified;
                context.SaveChanges();
                result = "true";
            }
            return result;
        }

        [HttpPost]
        public string saveCartDesc(int Id, string msg)
        {
            try
            {
                var result = "Something went wrong";
                using (var context = new SVIContext())
                {
                    var cartitem = context.Cart.FirstOrDefault(x => x.Id == Id);
                    cartitem.Description = msg;
                    context.Entry(cartitem).State = EntityState.Modified;
                    context.SaveChanges();
                    result = "true";
                }
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}