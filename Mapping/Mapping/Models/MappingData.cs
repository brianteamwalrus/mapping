using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Mapping.Models
{
    public static class MappingData
    {

        public static List<MapLocation> GetMarkers(int mapId)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("select * from MapMarkers where MapId=@dmapId", conn);
            Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
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

                    markers.Add(marker);
                }
            }
            conn.Close();
            return markers;
        }

        public static void AddMarker(int mapId, MapLocation marker)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MappingDatabase"].ConnectionString);
            SqlCommand Cmd = new SqlCommand("INSERT INTO MapMarkers (MapId,PlaceName,Address,Latitude,Longitude) VALUES (@dMapId,@dPlaceName,@dAddress,@dLatitude,@dLongitude)",conn);
            Cmd.Parameters.Add("@dMapId", System.Data.SqlDbType.Int).Value = mapId;
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