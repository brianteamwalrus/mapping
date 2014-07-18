using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mapping.Models
{
    public class MapGrid
    {
        public List<MapLocation> MarkerList { get; set; }
        public int CurrentPage;
        public int pageSize;
        public double TotalPages;
        public int sortBy;
        public bool isAsc;
        public string Search;
        public int isLastRecord;
        public int Count;

        public MapGrid()
        {
            MarkerList = new List<MapLocation>();

        }
    
    }
}