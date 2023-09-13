using Entities;
using Razorpay.Api;
using Services;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [Route("category/{category}/{CatId}")]
        public ActionResult Products(string CatId,string category)
        {
            try
            {
                int catId = Convert.ToInt32(HelperService.Instance.ConvertHexToString(CatId, System.Text.Encoding.Unicode));
                TempData["CatId"] = catId;
                var data = CategoryService.Instance.GetAllActiveSubCat(catId);
                return View(data);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        public ActionResult ProductsPartial(int? CatId, string Search, int? subCatId)
        {
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                var data = ProductService.Instance.GetActiveProByCat(CatId, Search, subCatId, loginUser);
                TempData["CatId"] = CatId;
                TempData["CatName"] = data != null && data.Count() > 0 ? data.FirstOrDefault().Category.Name : "";
                TempData["pCount"] = data.Count();
                return PartialView(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Route("Product/{name}/{Id}")]
        public ActionResult ProductView(string Id,string name)
        {
            try
            {
                int PId = Convert.ToInt32(HelperService.Instance.ConvertHexToString(Id, System.Text.Encoding.Unicode));
                var data = ProductService.Instance.GetProduct(PId);
                data.ProductImages = data.ProductImages.OrderByDescending(x => x.Default).ToList();
                TempData["ProId"] = PId;
                TempData["ProName"] = data.Name;
                return View(data);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }

        public ActionResult Seasonal()
        {
            try
            {
                var loginUser = UserService.Instance.GetCurrentUserLogin();
                var data = ProductService.Instance.getSeasonalProducts();
                return PartialView(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Decor()
        {
            try
            {
                using (var context = new SVIContext())
                {
                    var decdata = context.Products.Include(x=>x.ProductImages).FirstOrDefault(x => x.Name == "Decor");
                    return View(decdata);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}