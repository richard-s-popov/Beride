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
        public List<TripModel> trips { get; set; } 
    }
}