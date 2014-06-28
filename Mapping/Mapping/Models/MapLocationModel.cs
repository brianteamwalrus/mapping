﻿using System;
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

        [Required]
        [Display(Name = "Place Name")]
        public string PlaceName { get; set; }

        [Required]
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
                try
                {
                    LatLng = locationService.GetLatLongFromAddress(address);
                }
                catch (Exception ex)
                {}
            }
        }

        [Display(Name = "Location")]
        public MapPoint LatLng {get;set;}
    }

    public class MapLocationModel
    {
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

        public MapLocationModel()
        {
            mapLocations = new List<MapLocation>();
        }

        public List<MapLocation> GetMarkers()
        {
            mapLocations = MappingData.GetMarkers(1);
            return mapLocations;
        }

        public void AddMarker(MapLocation marker)
        {
            MappingData.AddMarker(1, marker);
            GetMarkers();
        }


    }

}