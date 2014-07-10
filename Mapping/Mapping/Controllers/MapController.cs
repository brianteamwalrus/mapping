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
            AddMarkerClass result = new AddMarkerClass();
            result.Status = false;

            bool validated = true;
            if (string.IsNullOrEmpty(PlaceName)) {
                validated = false;
                result.Text = "Place Name is a required field. ";
            }
            if (string.IsNullOrEmpty(Address))
            {
                validated = false;
                result.Text += "Address is a required field. ";
            }

            if (validated == true)
            {
                result.Location.PlaceName = PlaceName;
                result.Location.Address = Address;

                double latitude = 0;
                double.TryParse(Latitude, out latitude);
                double longitude = 0;
                double.TryParse(Longitude, out longitude);

                if (latitude == 0 && longitude == 0)
                {
                    result.Location.SetLocation();

                    if (result.Location.LatLng == null)
                    {
                        validated = false;
                        result.Text += "Unable to determine address from location. ";
                    }

                } else
                {
                    result.Location.LatLng.Latitude = latitude;
                    result.Location.LatLng.Longitude = longitude;
                }
            }

            if (validated == true)
            {
                result.Location.MarkerId = MappingData.AddMarker(User.Identity.Name, result.Location);
                result.Status = true;
            }

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jss.Serialize(result);
        }

        public ActionResult Create()
        {
            MapModel model = new MapModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MapModel model)
        {
            if (string.IsNullOrEmpty(model.mapDetail.MapName))
            {
                ModelState.AddModelError("mapDetail.MapName", "Map Name must be provided");
            }

            if (ModelState.IsValid)
            {
                model.mapDetail.MapIdentifier = System.Guid.NewGuid().ToString("N");
                int mapId = MappingData.CreateMap(model.mapDetail);
                if (mapId != 0)
                {
                    model.updateStatus = true;
                    model.mapDetail.MapId = mapId;
                    FormsAuthentication.SetAuthCookie(model.mapDetail.MapIdentifier, false);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Clear()
        {
            MappingData.DeleteAllMarkers(User.Identity.Name);
            return RedirectToAction("Index", "Map");
        }

        [Authorize]
        public ActionResult Edit()
        {
            MapModel model = MappingData.GetMap(User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MapModel model)
        {
            if (string.IsNullOrEmpty(model.mapDetail.MapName))
            {
                ModelState.AddModelError("mapDetail.MapName", "Map Name must be provided");
            }

            if (ModelState.IsValid)
            {
                model.mapDetail.MapIdentifier = User.Identity.Name;
                model.updateStatus = MappingData.EditMap(model.mapDetail);
                model.mapDetail.MapIdentifier = string.Empty;
            }
            return View(model);
        }


    }
}