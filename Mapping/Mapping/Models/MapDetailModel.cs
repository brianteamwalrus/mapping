using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mapping.Models
{
    public class MapDetailModel
    {
        [Display(Name = "Map ID")]
        public int MapId { get; set; }

        [Display(Name = "Map Name")]
        public string MapName { get; set; }

        [Display(Name = "Map Code")]
        public string MapCode { get; set; }

    }
}