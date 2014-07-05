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
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("select * from Maps where MapIdentifier=@dMapIdentifier", conn);
            Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = mapIdentifier;
            conn.Open();

            return GetMap(conn, Cmd);
        }

        public static MapModel GetMap(int mapId, string mapCode)
        {
            if (mapCode == null) mapCode = string.Empty;
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("select * from Maps where MapId=@dMapId and MapCode=@dMapCode", conn);
            Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
            Cmd.Parameters.Add("@dMapCode", System.Data.SqlDbType.NVarChar).Value = mapCode;
            conn.Open();

            return GetMap(conn, Cmd);
        }

        public static MapModel GetMap(int mapId)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("select * from Maps where MapId=@dMapId and MapViewAllowed=1", conn);
            Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
            conn.Open();

            return GetMap(conn, Cmd);
        }

        public static MapModel GetMap(SqlConnection conn, SqlCommand Cmd)
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
                    map.mapDetail.MapViewAllowed = (bool)Reader["MapViewAllowed"];
                }
            }
            conn.Close();
            return map;
        }


        public static List<MapLocation> GetMarkers(string mapIdentifier)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("select PlaceName,Address,Latitude,Longitude,MarkerId from MapMarkers INNER JOIN Maps ON Maps.MapId=MapMarkers.MapId where MapIdentifier=@dmapIdentifier", conn);
            Cmd.Parameters.Add("@dMapIdentifier", System.Data.SqlDbType.NVarChar).Value = mapIdentifier;
            conn.Open();

            List<MapLocation> markers = new List<MapLocation>();
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
            return markers;
        }

        public static void AddMarker(string mapIdentifier, MapLocation marker)
        {
            MapModel map = GetMap(mapIdentifier);
            
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("INSERT INTO MapMarkers (MapId,PlaceName,Address,Latitude,Longitude) VALUES (@dMapId,@dPlaceName,@dAddress,@dLatitude,@dLongitude)",conn);
            Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = map.mapDetail.MapId;
            Cmd.Parameters.Add("@dPlaceName", System.Data.SqlDbType.NVarChar).Value = marker.PlaceName;
            Cmd.Parameters.Add("@dAddress", System.Data.SqlDbType.NVarChar).Value = marker.Address;
            Cmd.Parameters.Add("@dLatitude", System.Data.SqlDbType.Float).Value = marker.LatLng.Latitude;
            Cmd.Parameters.Add("@dLongitude", System.Data.SqlDbType.Float).Value = marker.LatLng.Longitude;
            conn.Open();
            Cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}