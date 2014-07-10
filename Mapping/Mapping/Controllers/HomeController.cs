using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapping.Models;
using GoogleMaps.LocationServices;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.Routing;
using System.Web.UI;

namespace Mapping.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MapModel model)
        {
            model = MappingData.GetMap(model.mapDetail.MapId, model.mapDetail.MapCode);
            if (model != null)
            {
                FormsAuthentication.SetAuthCookie(model.mapDetail.MapIdentifier,false);
            }
            else
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Map"); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
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

    }
}