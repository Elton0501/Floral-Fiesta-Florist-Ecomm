using Entities;
using SV.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Web.Controllers
{
    public class NotificationactionController : Controller
    {
        [HttpPost]
        public ActionResult GetNotificationData()
        {
            try
            {
                var result = new List<Notification>();
                DateTime Current = DateTime.Now;
                using (var context = new SVIContext())
                {
                    result = context.Notification.ToList();
                    result = result.Select(x =>
                    {
                        x.TimeTrek = Current.Day > x.CreatedOn.Day ? Current.Day - x.CreatedOn.Day + " day ago" :
                        Current.Hour > x.CreatedOn.Hour ? Current.Hour - x.CreatedOn.Hour + " hours ago" :
                        Current.Minute > x.CreatedOn.Minute ? Current.Minute - x.CreatedOn.Minute + " minutes ago" : "";
                        return x;
                    }).ToList();
                }
                var serializer = new JavaScriptSerializer();
                var data = serializer.Serialize(result);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Redirect("~/Not_found.html");
            }
        }

        public void SaveNotification(Notification notification)
        {
            if (notification.Message != "" && notification.Message != null)
            {
                using (var context = new SVIContext())
                {
                    context.Notification.Add(notification);
                    context.SaveChanges();
                    MyHub objNotifHub = new MyHub();
                    objNotifHub.NotificationBrodcaast(notification);
                }
            }
        }
        [HttpPost]
        public string DeleteNotificationData(int Id)
        {
            var result = "false";
            if (Id > 0)
            {
                using (var context = new SVIContext())
                {
                    var data = context.Notification.FirstOrDefault(x => x.Id == Id);
                    context.Notification.Remove(data);
                    context.SaveChanges();
                    result = "true";
                }
            }
            return result;
        }
        [HttpPost]
        public string DeleteAllNotificationData()
        {
            var result = "false";
            using (var context = new SVIContext())
            {
                var data = context.Notification.ToList();
                context.Notification.RemoveRange(data);
                context.SaveChanges();
                result = "true";
            }
            return result;
        }
    }
}