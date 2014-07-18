using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapping.Models
{
    public class MapModel
    {
        public MapDetail mapDetail { get; set; }
        public MapLocation mapLocation { get; set; }
        public List<MapLocation> mapLocations { get; set; }
        public bool updateStatus { get; set; }
        public MapGrid mapGrid { get; set; }

        public string mapLocationsJson
        {
            get
            {
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                return jss.Serialize(mapLocations);
            }
        }

        public MapModel()
        {
            mapDetail = new MapDetail();
            mapLocation = new MapLocation();
            mapLocations = new List<MapLocation>();
            mapGrid = new MapGrid();
        }

    }
}