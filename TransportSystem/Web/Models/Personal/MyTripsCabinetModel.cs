using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransportSystem.Area.Web.Models.Requests;
using TransportSystem.Area.Web.Models.Trips;

namespace TransportSystem.Area.Web.Models.Personal
{
    public class MyTripsCabinetModel
    {
        public List<RequestModel> Requests { get; set; }

        public List<TripModel> Trips { get; set; }
    }
}