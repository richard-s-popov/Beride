using System;
using System.Collections.Generic;

namespace TransportSystem.Area.Web.Models.Trips
{
    public class TripModel
    {
        public int TripId { get; set; }

        public long RouteId { get; set; }

        public long TripDateId { get; set; }

        public bool IsDriver { get; set; }

        public bool? FreeDriver { get; set; }

        public int? SeatsNumber { get; set; }

        public int Cost { get; set; }

        public string StartPointFullName { get; set; }

        public string StartPointShortName { get; set; }

        public string EndPointFullName { get; set; }

        public string EndPointShortName { get; set; }

        public IEnumerable<string> MainRouteStr { get; set; }

        public IEnumerable<string> MainRouteShortStr { get; set; }

        public string MainRouteShortString { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartDateAt { get; set; }

        public DateTime StartDateTo { get; set; }

        public string StatusForUser { get; set; }

        public string OwnerName { get; set; }
    }

    public class SJSonModel
    {
        public SJSonModel(Dictionary<string, object> points)
        {
            if (points.ContainsKey("FullName"))
            {
                FullName = (string)points["FullName"];
            }
            if (points.ContainsKey("ShortName"))
            {
                ShortName = (string)points["ShortName"];
            }
            if (points.ContainsKey("Gid"))
            {
                Gid = (string)points["Gid"];
            }
        }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public string Gid { get; set; }
    }
}