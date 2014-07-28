using Mapping.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Mapping
{
    public static class MappingData
    {
        public static MapDetail GetMap(string mapIdentifier)
        {
            MapDetail map = null;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from Maps where MapIdentifier=@dMapIdentifier", conn))
                {
                    Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = mapIdentifier;
                    conn.Open();
                    map = GetMap(Cmd);
                    conn.Close();
                }
            }
            return map;
        }

        public static MapDetail GetMap(int mapId, string mapCode)
        {
            MapDetail map = null;
            if (mapCode == null) mapCode = string.Empty;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from Maps where MapId=@dMapId and MapCode=@dMapCode", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
                    Cmd.Parameters.Add("@dMapCode",System.Data.SqlDbType.NVarChar).Value = mapCode;
                    conn.Open();
                    map = GetMap(Cmd);
                    conn.Close();
                }
            }
            return map;
        }

        public static MapDetail GetMap(int mapId)
        {
            MapDetail map = null;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from Maps where MapId=@dMapId and MapPublicAllowed=1", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
                    conn.Open();
                    map = GetMap(Cmd);
                    conn.Close();
                }
            }
            return map;
        }

        public static MapDetail GetMap(SqlCommand Cmd)
        {
            MapDetail map = null;
            using (SqlDataReader Reader = Cmd.ExecuteReader())
            {
                while (Reader.Read())
                {
                    map = new MapDetail();
                    map.MapIdentifier = Reader["MapIdentifier"].ToString();
                    map.MapId = int.Parse(Reader["MapId"].ToString());
                    map.MapName = Reader["MapName"].ToString();
                    map.MapCode = Reader["MapCode"].ToString();
                    map.MapPublicAllowed = (bool)Reader["MapPublicAllowed"];
                }
            }
            return map;
        }


        public static List<MapLocation> GetMarkers(string mapIdentifier)
        {
            List<MapLocation> markers = new List<MapLocation>();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select PlaceName,Address,Latitude,Longitude,MarkerId from MapMarkers INNER JOIN Maps ON Maps.MapId=MapMarkers.MapId where MapIdentifier=@dmapIdentifier", conn))
                {
                    Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = mapIdentifier;
                    conn.Open();

                    MapLocation marker;
                    using (SqlDataReader Reader = Cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            marker = new MapLocation();
                            marker.PlaceName = HttpUtility.HtmlEncode(Reader["PlaceName"].ToString());
                            marker.Address = HttpUtility.HtmlEncode(Reader["Address"].ToString());
                            marker.LatLng.Latitude = (double)Reader["Latitude"];
                            marker.LatLng.Longitude = (double)Reader["Longitude"];
                            marker.MarkerId = (int)Reader["MarkerId"];
                            markers.Add(marker);
                        }
                    }
                    conn.Close();
                }
            }
            return markers;
        }

        public static int AddMarker(string mapIdentifier, MapLocation marker)
        {
            int markerId = 0;
            MapDetail map = GetMap(mapIdentifier);

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("INSERT INTO MapMarkers (MapId,PlaceName,Address,Latitude,Longitude) VALUES (@dMapId,@dPlaceName,@dAddress,@dLatitude,@dLongitude);SELECT SCOPE_IDENTITY();", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = map.MapId;
                    Cmd.Parameters.Add("@dPlaceName", System.Data.SqlDbType.NVarChar).Value = marker.PlaceName;
                    Cmd.Parameters.Add("@dAddress", System.Data.SqlDbType.NVarChar).Value = marker.Address;
                    Cmd.Parameters.Add("@dLatitude", System.Data.SqlDbType.Float).Value = marker.LatLng.Latitude;
                    Cmd.Parameters.Add("@dLongitude", System.Data.SqlDbType.Float).Value = marker.LatLng.Longitude;
                    conn.Open();
                    int.TryParse(Cmd.ExecuteScalar().ToString(), out markerId);
                    conn.Close();
                }
            }
            return markerId;
        }

        public static void DeleteAllMarkers(string mapIdentifier)
        {
            MapDetail map = GetMap(mapIdentifier);

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("DELETE FROM MapMarkers WHERE MapId=@dMapId;", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = map.MapId;
                    conn.Open();
                    Cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static bool DeleteMarker(string mapIdentifer, int markerId)
        {
            MapDetail map = GetMap(mapIdentifer);

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("DELETE FROM MapMarkers WHERE MapId=@dMapId AND MarkerId=@dMarkerId;", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = map.MapId;
                    Cmd.Parameters.Add("@dMarkerId", System.Data.SqlDbType.Int).Value = markerId;
                    conn.Open();
                    Cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return true;
        }


        public static int CreateMap(MapDetail map)
        {
            int mapId = 0;
            if (map.MapCode == null) map.MapCode = string.Empty;

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("INSERT INTO Maps (MapName,MapCode,MapIdentifier,MapPublicAllowed) VALUES (@dMapName,@dMapCode,@dMapIdentifier,@dMapPublicAllowed);SELECT SCOPE_IDENTITY();", conn))
                {
                    Cmd.Parameters.Add("@dMapName", System.Data.SqlDbType.NVarChar).Value = map.MapName;
                    Cmd.Parameters.Add("@dMapCode", System.Data.SqlDbType.NVarChar).Value = map.MapCode;
                    Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = map.MapIdentifier;
                    Cmd.Parameters.Add("@dMapPublicAllowed", System.Data.SqlDbType.Bit).Value = map.MapPublicAllowed;
                    conn.Open();
                    int.TryParse(Cmd.ExecuteScalar().ToString(), out mapId);
                    conn.Close();
                }
            }
            return mapId;
        }

        public static bool EditMap(MapDetail map)
        {
            if (map.MapCode == null) map.MapCode = string.Empty;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("UPDATE Maps SET MapName=@dMapName,MapCode=@dMapCode,MapPublicAllowed=@dMapPublicAllowed WHERE MapIdentifier=@dMapIdentifier;", conn))
                {
                    Cmd.Parameters.Add("@dMapName", System.Data.SqlDbType.NVarChar).Value = map.MapName;
                    Cmd.Parameters.Add("@dMapCode", System.Data.SqlDbType.NVarChar).Value = (object)map.MapCode ?? DBNull.Value;
                    Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = map.MapIdentifier;
                    Cmd.Parameters.Add("@dMapPublicAllowed", System.Data.SqlDbType.Bit).Value = map.MapPublicAllowed;
                    conn.Open();
                    Cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return true;
        }


        public static MapGrid GetMarkersForGrid(int mapId, int page = 1, int sortBy = 1, bool isAsc = true)
        {
            int pageSize = 20;
            MapGrid obj = new MapGrid();
            string sortColumn = string.Empty;

            #region SortingColumn
            switch (sortBy)
            {
                case 1:
                    if (isAsc)
                        sortColumn = "MarkerId";
                    else
                        sortColumn = "MarkerId Desc";
                    break;

                case 2:
                    if (isAsc)
                        sortColumn = "PlaceName";
                    else
                        sortColumn = "PlaceName Desc";
                    break;

                case 3:
                    if (isAsc)
                        sortColumn = "Address";
                    else
                        sortColumn = "Address Desc";
                    break;
            }
            #endregion


            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                string sqlQuery = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY "+sortColumn+") AS NUM, * FROM MapMarkers WHERE MapId=@dMapId) A WHERE NUM > @dLowRecord AND NUM < @dHighRecord;";
                using (SqlCommand Cmd = new SqlCommand(sqlQuery, conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
                    Cmd.Parameters.Add("@dLowRecord",System.Data.SqlDbType.Int).Value = ((page - 1) * pageSize);
                    Cmd.Parameters.Add("@dHighRecord", System.Data.SqlDbType.Int).Value = ((page * pageSize)+1);


                    conn.Open();
                    MapLocation marker;
                    using (SqlDataReader Reader = Cmd.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            marker = new MapLocation();
                            marker.PlaceName = HttpUtility.HtmlEncode(Reader["PlaceName"].ToString());
                            marker.Address = HttpUtility.HtmlEncode(Reader["Address"].ToString());
                            marker.LatLng.Latitude = (double)Reader["Latitude"];
                            marker.LatLng.Longitude = (double)Reader["Longitude"];
                            marker.MarkerId = (int)Reader["MarkerId"];
                            obj.MarkerList.Add(marker);
                        }
                    }
                    conn.Close();
                }

                using (SqlCommand Cmd = new SqlCommand("SELECT COUNT(*) AS NumberOfMarkers FROM MapMarkers WHERE MapId=@dMapId;", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
                    conn.Open();
                    int.TryParse(Cmd.ExecuteScalar().ToString(), out obj.Count);
                    conn.Close();
                }
            }


            obj.CurrentPage = page;
            obj.pageSize = pageSize;
            obj.sortBy = sortBy;
            obj.isAsc = isAsc;
            obj.TotalPages = Math.Ceiling((double)obj.Count / (double)obj.pageSize);

            if (obj.MarkerList.Count() <= pageSize)
                obj.isLastRecord = 2;

            if (obj.isLastRecord != 2)
            {
                if (obj.MarkerList.Count() <= pageSize)
                    obj.isLastRecord = 1;
                else
                    obj.isLastRecord = 0;
            }
            return obj;

        }


    }
}