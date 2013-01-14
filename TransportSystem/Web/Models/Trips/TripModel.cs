using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportSystem.Area.Web.Models
{
    public class TripModel
    {
        public DateTime Date { get; set; }

        public DateTime? ExpectedEndDate { get; set; }

        public bool IsDriver { get; set; }

        public bool? FreeDriver { get; set; }

        public int SeatsNumber { get; set; }

        public string StartPointFullName { get; set; }

        public string EndPointFullName { get; set; }
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