using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Saturn_BugTracker.Controllers
{
    public class HomeController : UniversalController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard", null);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ProfileSocial()
        {
            return View();
        }
    }
}