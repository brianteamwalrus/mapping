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

        [Display(Name = "Status")]
        public bool Status { get; set; }

        [Display(Name = "Status Text")]
        public string StatusText { get; set; }

        public MapLocation()
        {
            LatLng = new MapPoint();
        }

        public MapLocation(string placeName, string address, string latitude, string longitude)
        {
            LatLng = new MapPoint();

            try
            {
                Status = true;
                StatusText = string.Empty;

                PlaceName = placeName;
                Address = address;

                if (string.IsNullOrEmpty(PlaceName))
                {
                    Status = false;
                    StatusText = "Place Name is a required field. ";
                }
                if (string.IsNullOrEmpty(Address))
                {
                    Status = false;
                    StatusText += "Address is a required field. ";
                }

                if (Status == true)
                {
                    double latitudeValue = 0;
                    double.TryParse(latitude, out latitudeValue);
                    double longitudeValue = 0;
                    double.TryParse(longitude, out longitudeValue);

                    if (latitudeValue == 0 && longitudeValue == 0)
                    {
                        SetLocation();

                        if (LatLng == null)
                        {
                            Status = false;
                            StatusText += "Unable to determine address from location. ";
                        }

                    }
                    else
                    {
                        LatLng.Latitude = latitudeValue;
                        LatLng.Longitude = longitudeValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Status = false;
                StatusText += "Error validating location. ";
            }
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