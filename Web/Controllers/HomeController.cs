using DataModels;
using Entities;
using Microsoft.Ajax.Utilities;
using Services;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        HelperController helperController = new HelperController();
        public ActionResult Index()
        {
            try
            {
                var data = new HomeViewModel();
                data.Categories = CategoryService.Instance.GetAllActiveCategory();
                data.Tests = KeyService.Instance.Testemonails();
                return View(data);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        public ActionResult ProductSlider(bool product = false,bool bestSeller = false, bool featured = false)
        {
            TempData["catType"] = bestSeller == true ? "bestseller" : featured == true ? "featured" : "product";
            var resultData = new ProductSliderDataModel();
            IEnumerable<Cart> carts= null;
            var loginUser = UserService.Instance.GetCurrentUserLogin();
            var data = ProductService.Instance.GetAllActiveProducts();
            if (loginUser != null && loginUser != "")
            {
                carts = CartService.Instance.GetListUserCart(loginUser);
            }
            resultData.Product = data.Select(x =>
            {
                x.isAvailCart = carts != null && carts.Count() > 0 ? carts.FirstOrDefault(z => z.ProductId == x.Id) != null ? true : false:false;
                return x;
            }).ToList();

            if (bestSeller == true)
            {
                //data = OrdersService.Instance.GetBestSellers(resultData.Product);
                //resultData.Product = data.Count() > 5 ? data.DistinctBy(x => x.Id).OrderByDescending(x => x.BestSellerCount).ToList() : null;
                resultData.Product = data.Where(x => x.Isfeatured == true).ToList();

            }
            if (featured == true)
            {
                resultData.Product = data.Where(x=>x.Seasonal == true).ToList();
            }
            resultData.ProductCount = resultData.Product != null ? resultData.Product.ToList().Count() : 0;
            return PartialView(resultData);
        }

        //About , contact and newsletter
        [Route("about-us")]
        public ActionResult About()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddContactForm(Contact model)
        {
            try
            {
                var result = "* All fields are required to fill.";
                if (model.Email != string.Empty && model.Email != null && model.Name != null && model.Name != string.Empty && model.Message != null && model.Message != string.Empty)
                {
                    using (var context = new SVIContext())
                    {
                        model.CreatedOn = DateTime.Now;
                        context.Contact.Add(model);
                        context.SaveChanges();
                    }
                    result = "true";
                    if (result == "true")
                    {
                        string message = "<p>Hi " + model.Name+ "</p></br>"
                        +"<p>We received your email and will get back to you with a response as soon as possible, that’s usually within a couple of hours. Evenings and weekends may take us a little bit longer.</p></br>"
                            +"<p>If you have any additional information that you think will help us to assist you, please feel free to reply to this email.</p></br>"
                            + "</br>Team Froral Fiesta";
                        string subject = "Thanks so much for reaching out to Floral Fiesta";
                        string Head = "We got it";
                        helperController.templateEmail(model.Email, subject, Head, message);

                        string message1 = model.Message;
                        string subject1 = "Contact form";
                        string Head1 = "Contact form" + model.Name;
                        helperController.templateEmail(ConfigurationManager.AppSettings["email"].ToString(), subject1, Head1, message1);
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        [HttpPost]
        public ActionResult AddsubscribeForm(Newsletter model)
        {
            try
            {
                var result = "* All fields are required to fill.";
                if (model.Email != string.Empty && model.Email != null && model.Name != null && model.Name != string.Empty)
                {
                    using (var context = new SVIContext())
                    {
                        var Existdata = context.Newsletter.FirstOrDefault(x => x.Email == model.Email);
                        if (Existdata == null)
                        {
                            model.CreatedOn = DateTime.Now;
                            context.Newsletter.Add(model);
                            context.SaveChanges();
                            result = "true";
                            if (result == "true")
                            {
                                string message = "<p>Thank you for subscribibg to our newsletter" + model.Name + "</p>";
                                string subject = "Floral Fiesta: Newsletter Subscribed";
                                string Head = "Newsletter Subscribed";
                                helperController.templateEmail(model.Email, subject, Head, message);
                            }
                        }
                        else
                        {
                            result = "* You have already register.";
                        }
                    }
                }
                return Json(result);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
    }
}