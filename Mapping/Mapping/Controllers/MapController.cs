using Mapping.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);
            model.mapLocations = MappingData.GetMarkers(User.Identity.Name);

            return View(model);
        }

        public ActionResult Public(string id)
        {
            MapModel model = null;

            int mapId = 0;
            int.TryParse(id, out mapId);

            if (mapId > 0)
            {
                model = new MapModel();
                model.mapDetail = MappingData.GetMap(mapId);
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
            MapLocation result = new MapLocation(PlaceName, Address, Latitude, Longitude);
            if (result.Status == true)
            {
                result.MarkerId = MappingData.AddMarker(User.Identity.Name, result);
            }

            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jss.Serialize(result);
        }

        // Called from AJAX javascript
        [Authorize]
        [HttpPost]
        public string DeleteMarker(string MarkerId)
        {
            bool result = false;
            int markerId = 0;
            int.TryParse(MarkerId, out markerId);
            if (markerId!=0)
            {
                result = MappingData.DeleteMarker(User.Identity.Name, markerId);
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
            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);
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

        [Authorize]
        public ActionResult Grid(int page = 1, int sortBy = 2, bool isAsc = true)
        {
            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);
            model.mapGrid = MappingData.GetMarkersForGrid(model.mapDetail.MapId, page, sortBy, isAsc);
            return View(model);
        }

        [Authorize]
        public ActionResult Import()
        {
            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(HttpPostedFileBase file, bool? onlyErrors)
        {
            MapModel model = new MapModel();
            model.mapDetail = MappingData.GetMap(User.Identity.Name);

            try
            {
                if (file.ContentLength > 0)
                {
                    //var fileName = Path.GetFileName(file.FileName);
                    //var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                    //file.SaveAs(path);

                    var placeNameColumn = -1;
                    var addressColumn = -1;
                    var latitudeColumn = -1;
                    var longitudeColumn = -1;
                    var fileRecordCount = 0;

                    var reader = new StreamReader(file.InputStream);
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(new string[] { @""",""" }, StringSplitOptions.None);

                        if (fileRecordCount == 0)
                        {
                            for (var i = 0; i < values.Length; i++)
                            {
                                if (values[i].Contains("PlaceName"))
                                {
                                    placeNameColumn = i;
                                }
                                if (values[i].Contains("Address"))
                                {
                                    addressColumn = i;
                                }
                                if (values[i].Contains("Latitude"))
                                {
                                    latitudeColumn = i;
                                }
                                if (values[i].Contains("Longitude"))
                                {
                                    longitudeColumn = i;
                                }
                            }
                        }
                        else
                        {
                            string placeNameData = string.Empty;
                            if (placeNameColumn >= 0) placeNameData = values[placeNameColumn].Trim(new Char[] { '"' });

                            string addressData = string.Empty;
                            if (addressColumn >= 0) addressData = values[addressColumn].Trim(new Char[] { '"' });

                            string latitudeData = string.Empty;
                            if (latitudeColumn >= 0) latitudeData = values[latitudeColumn].Trim(new Char[] { '"' });

                            string longitudeData = string.Empty;
                            if (longitudeColumn >= 0) longitudeData = values[longitudeColumn].Trim(new Char[] { '"' });

                            MapLocation result = new MapLocation(placeNameData,
                                addressData,
                                latitudeData,
                                longitudeData);
                            if (result.Status == true)
                            {
                                result.MarkerId = MappingData.AddMarker(User.Identity.Name, result);
                                if (result.MarkerId == 0)
                                {
                                    result.Status = false;
                                    result.StatusText = "Error saving marker";
                                }
                            }

                            if (result.Status == true)
                            {
                                result.StatusText = "<p class='text-success'>Success</p>";
                                model.CountSuccess++;
                                if (onlyErrors == null || onlyErrors == false) model.mapLocations.Add(result);
                            } else
                            {
                                model.CountFailure++;
                                model.mapLocations.Add(result);
                            }
                        }
                        fileRecordCount++;
                    }
                    model.CountTotal = model.CountSuccess + model.CountFailure;
                }
            }
            catch (Exception ex)
            { }
            return View(model);
        }

    }
}