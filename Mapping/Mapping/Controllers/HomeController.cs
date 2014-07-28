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
            //FormsAuthentication.SignOut();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MapModel model)
        {
            model.mapDetail = MappingData.GetMap(model.mapDetail.MapId, model.mapDetail.MapCode);
            if (model != null && model.mapDetail != null)
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
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult About()
        {
            ViewBag.Message = "TWMapping is currently in BETA release.";
            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);

            return View(model);
        }

    }
}