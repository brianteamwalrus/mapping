using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapping
{
    public class AddMarkerClass
    {
        public MapLocation Location { get; set; }
        public bool Status { get; set; }
        public string Text { get; set; }

        public AddMarkerClass()
        {
            Location = new MapLocation();
        }

    }
}