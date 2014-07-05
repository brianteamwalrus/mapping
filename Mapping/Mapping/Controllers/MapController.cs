using Mapping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Mapping.Controllers
{
    public class MapController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            MapModel model = MappingData.GetMap(User.Identity.Name);
            model.mapLocations = MappingData.GetMarkers(User.Identity.Name);

            return View(model);
        }

        public ActionResult MapView(string id)
        {
            MapModel model = null;

            int mapId = 0;
            int.TryParse(id, out mapId);

            if (mapId > 0)
            {
                model = MappingData.GetMap(mapId);
            }
            if (model != null && model.mapDetail != null)
            {
                model.mapLocations = MappingData.GetMarkers(model.mapDetail.MapIdentifier);
            } else
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Index",model);
        }

        // Called from AJAX javascript
        [Authorize]
        [HttpPost]
        public string AddMarker(string PlaceName, string Address, string Latitude, string Longitude)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(PlaceName) && !string.IsNullOrEmpty(Address))
            {
                MapLocation location = new MapLocation();
                location.PlaceName = PlaceName;
                location.Address = Address;
                if (string.IsNullOrEmpty(Latitude) || string.IsNullOrEmpty(Longitude))
                {
                    location.SetLocation();
                }
                else
                {
                    location.LatLng.Latitude = double.Parse(Latitude);
                    location.LatLng.Longitude = double.Parse(Longitude);
                }

                if (location.LatLng != null)
                {
                    MappingData.AddMarker(User.Identity.Name, location);
                    System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                    result = jss.Serialize(location);
                }
            }
            return result;
        }

        public ActionResult Create()
        {
            return View();
        }

    }
}