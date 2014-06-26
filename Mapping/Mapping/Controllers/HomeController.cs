using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapping.Models;
using GoogleMaps.LocationServices;
using System.Data.SqlClient;

namespace Mapping.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            MappingModel model = new MappingModel();
            model.GetMarkers(1);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MappingModel model)
        {
            model.AddMarker(model.mapLocation);
            return View(model);
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