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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MapDetailModel model)
        {
            if (model.MapId == 0) return RedirectToAction("Index", "Home");

            Session.Add("MapId",model.MapId);
            return RedirectToAction("Map", "Home"); 
        }

        public ActionResult Map()
        {
            MapLocationModel model = new MapLocationModel();
            model.GetMarkers();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Map(MapLocationModel model)
        {

            if (model.mapLocation!= null && model.mapLocation.LatLng != null)
            {
                model.AddMarker(model.mapLocation);
            }
            else
            {
                ModelState.AddModelError("mapLocation.Address", "Address not found");
                model.GetMarkers();
            }

            return View(model);
        }

        [HttpPost]
        public bool AddMarker(string PlaceName, string Address)
        {
            bool result = false;
            if (PlaceName == "home") result = true;

            return result;
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