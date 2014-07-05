using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mapping
{
    public class MapLocation
    {
        [Required]
        [Display(Name = "Place Name")]
        public string PlaceName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Location")]
        public MapPoint LatLng { get; set; }

        public int MarkerId { get; set; }

        public MapLocation()
        {
            LatLng = new MapPoint();
        }

        public void SetLocation()
        {
            LatLng = null;
            var locationService = new GoogleLocationService();
            try
            {
                LatLng = locationService.GetLatLongFromAddress(Address);
            }
            catch (Exception ex)
            { }
        }
    }
}