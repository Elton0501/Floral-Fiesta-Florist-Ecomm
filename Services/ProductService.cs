using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Services
{
    public class ProductService
    {
        #region singleton
        public static ProductService Instance
        {
            get
            {
                if (instance == null) instance = new ProductService();
                return instance;
            }
        }
        private static ProductService instance { get; set; }

        public ProductService()
        {
        }
        #endregion

        public IEnumerable<Product> GetAllActiveProducts()
        {
            using (var context = new SVIContext())
            {
                var data = context.Products.Include(x => x.ProductImages).Include(x => x.SubCategory).ToList();
                var Categorydata = context.Categories.ToList();
                data = data.Select(x =>
                {
                    x.Category = Categorydata.FirstOrDefault(y=>y.Id == x.CategoryId);
                    x.DefaultImage = x.ProductImages.Count > 0 ? x.ProductImages.FirstOrDefault(z => z.Default == true) != null ? x.ProductImages.FirstOrDefault(z => z.Default == true).Image : x.ProductImages.FirstOrDefault().Image : "";
                    return x;
                }).ToList();
                data = data.Where(x => x.Status == true && x.SubCategory.Status == true && x.Category.Status == true).ToList();
                return data;
            }
        }
        public IEnumerable<Product> GetAllProducts()
        {
            using (var context = new SVIContext())
            {
                return context.Products.ToList();
            }
        }
        public Product GetProduct(int id)
        {
            using(var context = new SVIContext())
            {
                IEnumerable<Cart> carts = null;
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                if (loginUser != null && loginUser != "")
                {
                    carts = CartService.Instance.GetListUserCart(loginUser);
                }
                var userProCart = carts != null && carts.Count() > 0 ? carts.FirstOrDefault(z => z.ProductId == id): null;
                var data = context.Products.Include(x=>x.ProductImages).Include(x=>x.SubCategory).FirstOrDefault(x => x.Id == id);
                data.isAvailCart = userProCart != null ? true : false;
                data.cartId = userProCart != null ? userProCart.Id : 0;
                return data;
            }
        }

        public IEnumerable<Product> GetActiveProByCat(int? catId, string Search,int? subCatId, string loginUser)
        {
            using (var context = new SVIContext())
            {
                IEnumerable<Cart> carts = null;
                if (loginUser != null && loginUser != "")
                {
                    carts = CartService.Instance.GetListUserCart(loginUser);
                }
                var data = context.Products.Include(x => x.ProductImages).Include(x => x.SubCategory).ToList();
                var Categorydata = context.Categories.ToList();
                data = data.Select(x =>
                {
                    x.isAvailCart = carts != null && carts.Count() > 0 ? carts.FirstOrDefault(z => z.ProductId == x.Id) != null ? true : false: false;
                    x.Category = Categorydata.FirstOrDefault(y => y.Id == x.CategoryId);
                    x.DefaultImage = x.ProductImages.Count > 0 ? x.ProductImages.FirstOrDefault(z => z.Default == true) != null ? x.ProductImages.FirstOrDefault(z => z.Default == true).Image : x.ProductImages.FirstOrDefault().Image : "";
                    return x;
                }).ToList();
                data = data.Where(x => x.Status == true && x.SubCategory.Status == true && x.Category.Status == true && x.CategoryId == catId).ToList();
                if (Search != "" && Search != null)
                {
                    data = data.Where(x => x.Name.ToLower().Contains(Search.ToLower())).ToList();
                }
                if (subCatId.HasValue && subCatId.Value > 0)
                {
                    data = data.Where(x=>x.SubCategoryId == subCatId).ToList();
                }
                return data;
            }
        }

        public List<Product> getSeasonalProducts()
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var products = context.Products.Where(x => x.Seasonal == true).ToList();
                    return products;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
