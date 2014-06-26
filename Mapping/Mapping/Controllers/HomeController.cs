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
            model.GetMarkers();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(MappingModel model)
        {

            if (model.mapLocation!= null && model.mapLocation.LatLng != null)
            {
                model.AddMarker(model.mapLocation);
                model.mapLocation = new MapLocation();
                model.mapLocation.PlaceName = "";
            }
            else
            {
                if (model.mapLocation!= null) ModelState.AddModelError("mapLocation.Address", "Address not found");
                model.GetMarkers();
            }

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