using DataModels;
using Entities;
using Services;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        HelperController helperController = new HelperController();
        // GET: Account
        [Route("register")]
        public ActionResult Register(string url)
        {
            TempData["URL"] = url;
            return View();
        }
        [HttpPost]
        public ActionResult Register(string Mobile, string Name, string Email, string Password)
        {
            try
            {
                var Result = false;
                var Msg = "Something went wrong!";
                if (Mobile != null && Name != null && Email != null && Password != null)
                {
                    var user = new User();
                    user.Name = Name;
                    user.MobileNumber = Mobile;
                    user.Email = Email;
                    user.CreatedOn = DateTime.Now;
                    user.Password = Password;
                    user.Varified = false;
                    user.RoleId = AccountService.Instance.GetRoleIdByName(Constant.Role.User);
                    var result = AccountService.Instance.Regiter(user);
                    Result = result.Result;
                    Msg = result.Messsage;
                    if (result.Result == false)
                    {
                        TempData["Message"] = "Something went wrong";
                    }
                    else
                    {
                        //if (Request.Cookies[Constant.WebCookie] != null)
                        //{
                        //    HttpCookie oldcookie = new HttpCookie(Constant.WebCookie);
                        //    oldcookie.Expires = DateTime.Now.AddDays(-1);
                        //    Response.Cookies.Add(oldcookie);
                        //}
                        //HttpCookie cookie = new HttpCookie(Constant.WebCookie);
                        //cookie.Expires = DateTime.Now.AddDays(300);
                        //var cookiedata = user.Email;
                        //cookie.Value = HelperService.Instance.Encrypt(cookiedata);
                        //Response.Cookies.Set(cookie);
                        if (Password != null && Password != "")
                        {
                            string link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ConfirmUserEmail?email=" + HelperService.Instance.Encrypt(Email);
                            string subject = "Floral Fiesta: Email Confirmation";
                            string Head = "Please confirm your email here.";
                            string message = "<p>Thank you for registering at Floral Fiesta.</p></br>"
                                + "<p>Please click below link to confirm your email and enjoy a great shopping expierience with us.</p>" +
                                "<a href=\"" + link + "\">click to confirm your email</a>";
                            helperController.templateEmail(Email, subject, Head, message);
                        }
                        var notification = new Notification();
                        notification.CreatedOn = DateTime.Now;
                        notification.isMultipleUser = false;
                        notification.AnchorLink = "/Admin/UserList";
                        notification.Status = Constant.NotificationConstant.unread;
                        notification.Message = string.Format(Constant.NotificationConstant.AddNewUser, Name, Email);
                        var notificationAction = new NotificationactionController();
                        notificationAction.SaveNotification(notification);
                    }
                }
                return Json(new { result = Result, msg = Msg });
            }
            catch (Exception)
            {
                return Redirect("~/not_found.html");
            }
        }
        [Route("login")]
        public ActionResult Login(string url, string msg)
        {
            TempData["ConfirmEmail"] = msg;
            ViewBag.Url = url;
            return View();
        }
        public ActionResult PartialLogin(string url, string msg)
        {
            TempData["ConfirmEmail"] = msg;
            ViewBag.Url = url;
            return PartialView();
        }

        [HttpPost]
        public ActionResult Login(string Email, string Password, string Url)
        {
            try
            {
                var result = AccountService.Instance.Login(Email, Password);
                if (result.Result == true)
                {
                    if (Request.Cookies[Constant.WebCookie] != null)
                    {
                        HttpCookie oldcookie = new HttpCookie(Constant.WebCookie);
                        oldcookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(oldcookie);
                    }
                    if (result.Value2 == "Admin")
                    {
                        Session["Admin"] = Email;
                        Url = "/Admin/Index";
                    }
                    else
                    {
                        HttpCookie cookie = new HttpCookie(Constant.WebCookie);
                        cookie.Expires = DateTime.Now.AddDays(300);
                        var cookiedata = Email;
                        cookie.Value = HelperService.Instance.Encrypt(cookiedata);
                        Response.Cookies.Set(cookie);
                    }
                }
                return Json(new { result = result.Result, msg = result.Messsage, url = Url });
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }
        [Route("forgotpassword")]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            try
            {
                var result = AccountService.Instance.ForgotPassword(Email);
                if (result.Result == true)
                {
                    string link = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) + "/Account/ResetPassword?email=" + HelperService.Instance.Encrypt(Email);
                    string subject = "Floral Fiesta: Reset Password";
                    string Head = "Please reset your Password here.";
                    string message = "<p>Please click on the link below to reset your password and continue shopping with us.</p>" +
                        "<a href=\"" + link + "\">click to reset your password</a>";
                    helperController.templateEmail(Email, subject, Head, message);
                }
                return Json(new { result = result.Result, msg = result.Messsage });
            }
            catch (Exception)
            {
                return Redirect("~/not_found.html");
            }
        }
        public ActionResult ConfirmUserEmail(string email)
        {
            var Email = HelperService.Instance.Decrypt(email);
            using (var context = new SVIContext())
            {
                var result = context.Users.FirstOrDefault(x => x.Email == Email);
                if (result != null)
                {
                    result.Varified = true;
                    context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    TempData["ConfirmEmail"] = "Email confirmed successfully. Please login to continue";
                }
            }
            return RedirectToAction("Login", "Account" , new { msg = "Email confirmed successfully. Please login to continue" });
        }
        [Route("logout")]
        public ActionResult Logout()
        {
            try
            {
                if (Request.Cookies[Constant.WebCookie] != null)
                {
                    HttpCookie oldcookie = new HttpCookie(Constant.WebCookie);
                    oldcookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(oldcookie);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return Redirect("~/not_found.html");
            }
        }
        public ActionResult AdminLogout()
        {
            try
            {
                Session.Remove("Admin");
                return RedirectPermanent("/Home/Index");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Error", new { CloseWeb = false, maintainance = false, NotFound = true });
            }
        }
        public ActionResult ResetPassword(string email)
        {
            var Email = HelperService.Instance.Decrypt(email);
            ViewBag.Email = Email;
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string email,string password)
        {
            bool Result = false;
            var Messsage = "Something went wrong!";
            using (var context = new SVIContext())
            {
                var result = context.Users.FirstOrDefault(x => x.Email == email);
                if (result != null)
                {
                    result.Varified = true;
                    result.Password = HelperService.Instance.Encrypt(password);
                    context.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    Result = true;
                    Messsage = "Reset password successfully. Please login to continue";
                }
            }
            return Json(new { result = Result, msg = Messsage });
        }
    }
}