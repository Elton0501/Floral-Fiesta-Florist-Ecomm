using DataModels;
using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Remoting.Contexts;

namespace Services
{
    public class CartService
    {
        #region singleton
        public static CartService Instance
        {
            get
            {
                if (instance == null) instance = new CartService();
                return instance;
            }
        }
        private static CartService instance { get; set; }

        public CartService()
        {
        }
        #endregion

        public CartDataModel AddInCart(int ProductId, string userIdentity, int Quantity)
        {
            using (var context = new SVIContext())
            {
                CartDataModel model = new CartDataModel();
                model.result = false;
                var existincart = context.Cart.FirstOrDefault(x => x.CreatedBy == userIdentity && x.ProductId == ProductId);
                if (existincart == null)
                {
                    var cart = new Cart();
                    cart.ProductId = ProductId;
                    cart.CreatedBy = userIdentity;
                    cart.Quantity = Quantity > 0 ? Quantity : 1;
                    cart.CreatedOn = DateTime.Now;
                    context.Cart.Add(cart);
                    context.SaveChanges();
                    model.CartId = cart.Id.ToString();
                }
                else
                {
                    existincart.Quantity = existincart.Quantity + 1;
                    context.Entry(existincart).State= System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    model.CartId = existincart.Id.ToString();
                }
                var cartdata = context.Cart.Where(x => x.CreatedBy == userIdentity).ToList();
                cartdata = cartdata.Select(x =>
                {
                    x.Product = context.Products.FirstOrDefault(y => y.Id == x.ProductId);
                    return x;
                }).ToList();
                model.Count = cartdata.Select(x => x.Quantity).Sum().ToString();
                model.TotalAmount = cartdata.Count > 0 ? cartdata.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum() : 0;
                model.result = true;
                return model;
            }
        }

        public CartDataModel RemoveInCart(int cartItemId,string uId , bool homePage)
        {
            CartDataModel model = new CartDataModel();
            model.result = false;
            using (var context = new SVIContext())
            {
                var cartdata = context.Cart.FirstOrDefault(x => x.Id == cartItemId);
                if (homePage == true)
                {
                    cartdata = context.Cart.FirstOrDefault(x => x.ProductId == cartItemId && x.CreatedBy == uId);
                }
                context.Cart.Remove(cartdata);
                context.SaveChanges();
                var cartItemdata = context.Cart.Where(x=>x.CreatedBy == uId).ToList();
                cartItemdata = cartItemdata.Select(x =>
                {
                    x.Product = context.Products.FirstOrDefault(y => y.Id == x.ProductId);
                    return x;
                }).ToList();
                model.Count = cartItemdata.Select(x => x.Quantity).Sum().ToString();
                model.TotalAmount = cartItemdata.Count > 0 ? cartItemdata.Select(x => (x.Product.DiscountPrice > 0 ? x.Product.DiscountPrice : x.Product.Price) * x.Quantity).Sum() : 0;
                model.TotalwithoutDisc = cartItemdata.Count > 0 ? cartItemdata.Select(x => x.Product.Price * x.Quantity).Sum() : 0;
                model.result = true;
                return model;
            }
        }

        public Cart GetUserCart(string UserIdentity)
        {
            using (var context = new SVIContext())
            {
                var data = context.Cart.FirstOrDefault(x => x.CreatedBy == UserIdentity);
                data.Product = context.Products.Include(x=>x.ProductImages).FirstOrDefault(x=>x.Id == data.ProductId);
                if (data.Product != null)
                {
                    data.Product.DefaultImage = data.Product.ProductImages.Count > 0 ? data.Product.ProductImages.FirstOrDefault(z => z.Default == true) != null ? data.Product.ProductImages.FirstOrDefault(z => z.Default == true).Image : data.Product.ProductImages.FirstOrDefault().Image : "";
                }
                return data != null ? data : null;
            }
        }
        public IEnumerable<Cart> GetListUserCart(string UserIdentity)
        {
            using (var context = new SVIContext())
            {
                var data = context.Cart.Where(x => x.CreatedBy == UserIdentity).ToList();
                data = data.Select(y =>
                {
                    y.Product = context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == y.ProductId);
                    y.Product.DefaultImage = y.Product.ProductImages.Count > 0 ? y.Product.ProductImages.FirstOrDefault(z => z.Default == true) != null ? y.Product.ProductImages.FirstOrDefault(z => z.Default == true).Image : y.Product.ProductImages.FirstOrDefault().Image : "";
                    return y;
                }).ToList();
                return data;
            }
        }
        public int GetUserCartCount(string UserIdentity)
        {
            using (var context = new SVIContext())
            {
                var data = context.Cart.Where(x => x.CreatedBy == UserIdentity).ToList().Select(x=>x.Quantity).Sum();
                return data;
            }
        }

        

        public List<Key> SlotNumber(int Time)
        {
            List<Key> result = new List<Key>();
            using (var context =  new SVIContext())
            {
                var slots = context.Keys.Where(x => x.Type == "Slot").ToList();
                if (Time < 11)
                {
                    var data = slots.Where(x => x.Name != "1").ToList();
                    result.AddRange(data);
                }
                else if (Time < 14)
                {
                    var data = slots.Where(x => x.Name != "1" && x.Name != "2").ToList();
                    result.AddRange(data);
                }
                else if (Time < 17)
                {
                    var data = slots.Where(x => x.Name != "1" && x.Name != "2" && x.Name != "3").ToList();
                    result.AddRange(data);
                }
            }
            return result;
        }
    }
}
