using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mapping
{
    public class MapDetail
    {
        [Required]
        [Display(Name = "Map Identifier")]
        public string MapIdentifier { get; set; }

        [Display(Name = "Map Id")]
        public int MapId { get; set; }

        [Display(Name = "Map Name")]
        public string MapName { get; set; }

        [Display(Name = "Map Code")]
        public string MapCode { get; set; }

        [Display(Name = "Allow View Without Code")]
        public bool MapViewAllowed { get; set; }

    }
}