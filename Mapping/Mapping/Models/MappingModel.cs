using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GoogleMaps.LocationServices;
using System.Data.SqlClient;

namespace Mapping.Models
{
    public class MapLocation
    {
        private string address;


        [Display(Name = "Place Name")]
        public string PlaceName { get; set; }

        [Display(Name = "Address")]
        public string Address 
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                var locationService = new GoogleLocationService();
                LatLng = locationService.GetLatLongFromAddress(address);
            }
        }

        [Display(Name = "Location")]
        public MapPoint LatLng {get;set;}
    }


    public class MappingModel
    {
        public int MapId { get; set; }
        public MapLocation mapLocation { get; set; }
        private List<MapLocation> mapLocations { get; set; }
        public string mapLocationsJson
        {
            get
            {
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                return jss.Serialize(mapLocations);
            }
        }

        public MappingModel()
        {
            mapLocations = new List<MapLocation>();
        }

        public List<MapLocation> GetMarkers(int mapId)
        {
            mapLocations = MappingData.GetMarkers(mapId);
            return mapLocations;
        }

        public void AddMarker(MapLocation marker)
        {
            MappingData.AddMarker(MapId, marker);
            GetMarkers(MapId);
        }


    }

}