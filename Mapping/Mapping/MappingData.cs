using Mapping.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Mapping
{
    public static class MappingData
    {
        public static MapModel GetMap(string mapIdentifier)
        {
            MapModel map = null;
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

        public static MapModel GetMap(int mapId, string mapCode)
        {
            MapModel map = null;
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

        public static MapModel GetMap(int mapId)
        {
            MapModel map = null;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("select * from Maps where MapId=@dMapId and MapViewAllowed=1", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
                    conn.Open();
                    map = GetMap(Cmd);
                    conn.Close();
                }
            }
            return map;
        }

        public static MapModel GetMap(SqlCommand Cmd)
        {
            MapModel map = null;
            using (SqlDataReader Reader = Cmd.ExecuteReader())
            {
                while (Reader.Read())
                {
                    map = new MapModel();
                    map.mapDetail.MapIdentifier = Reader["MapIdentifier"].ToString();
                    map.mapDetail.MapId = int.Parse(Reader["MapId"].ToString());
                    map.mapDetail.MapName = Reader["MapName"].ToString();
                    map.mapDetail.MapCode = Reader["MapCode"].ToString();
                    map.mapDetail.MapViewAllowed = (bool)Reader["MapViewAllowed"];
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
                            marker.PlaceName = Reader["PlaceName"].ToString();
                            marker.Address = Reader["Address"].ToString();
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
            MapModel map = GetMap(mapIdentifier);

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("INSERT INTO MapMarkers (MapId,PlaceName,Address,Latitude,Longitude) VALUES (@dMapId,@dPlaceName,@dAddress,@dLatitude,@dLongitude);SELECT SCOPE_IDENTITY();", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = map.mapDetail.MapId;
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
            MapModel map = GetMap(mapIdentifier);

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("DELETE FROM MapMarkers WHERE MapId=@dMapId;", conn))
                {
                    Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = map.mapDetail.MapId;
                    conn.Open();
                    Cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static int CreateMap(MapDetail map)
        {
            int mapId = 0;
            if (map.MapCode == null) map.MapCode = string.Empty;

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString))
            {
                using (SqlCommand Cmd = new SqlCommand("INSERT INTO Maps (MapName,MapCode,MapIdentifier,MapViewAllowed) VALUES (@dMapName,@dMapCode,@dMapIdentifier,@dMapViewAllowed);SELECT SCOPE_IDENTITY();", conn))
                {
                    Cmd.Parameters.Add("@dMapName", System.Data.SqlDbType.NVarChar).Value = map.MapName;
                    Cmd.Parameters.Add("@dMapCode", System.Data.SqlDbType.NVarChar).Value = map.MapCode;
                    Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = map.MapIdentifier;
                    Cmd.Parameters.Add("@dMapViewAllowed", System.Data.SqlDbType.Bit).Value = map.MapViewAllowed;
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
                using (SqlCommand Cmd = new SqlCommand("UPDATE Maps SET MapName=@dMapName,MapCode=@dMapCode,MapViewAllowed=@dMapViewAllowed WHERE MapIdentifier=@dMapIdentifier;", conn))
                {
                    Cmd.Parameters.Add("@dMapName", System.Data.SqlDbType.NVarChar).Value = map.MapName;
                    Cmd.Parameters.Add("@dMapCode", System.Data.SqlDbType.NVarChar).Value = (object)map.MapCode ?? DBNull.Value;
                    Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = map.MapIdentifier;
                    Cmd.Parameters.Add("@dMapViewAllowed", System.Data.SqlDbType.Bit).Value = map.MapViewAllowed;
                    conn.Open();
                    Cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            return true;
        }


    }
}