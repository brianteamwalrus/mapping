using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mapping
{
    // Details of the map
    public class MapDetail
    {
        [Display(Name = "Map Identifier")]
        public string MapIdentifier { get; set; }

        [Display(Name = "Map Id")]
        public int MapId { get; set; }

        [Display(Name = "Map Name")]
        public string MapName { get; set; }

        [Display(Name = "Password")]
        public string MapCode { get; set; }

        [Display(Name = "Allow Public")]
        public bool MapPublicAllowed { get; set; }

    }
}