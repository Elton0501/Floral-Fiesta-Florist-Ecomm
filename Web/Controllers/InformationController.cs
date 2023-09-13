using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class InformationController : Controller
    {
        // GET: Information
        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Returns()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }
    }
}