using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransportSystem.Domain;

namespace TransportSystem.Area.Web.Models.Trips
{
    public class TripsListModel
    {
        public TripsListModel()
        {
            trips = new List<TripModel>();
        }

        public string StartPointGid { get; set; }

        public string StartPointFullName { get; set; }

        public string StartPointShortName { get; set; }

        public string EndPointGid { get; set; }

        public string EndPointFullName { get; set; }

        public string EndPointShortName { get; set; }

        public string DateAt { get; set; }

        public string DateTo { get; set; }

        public List<TripModel> trips { get; set; } 
    }
}