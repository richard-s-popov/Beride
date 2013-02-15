using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportSystem.Area.Web.Models.Requests
{
    public class RequestModel
    {
        public string UserFullName { get; set; }

        public string UserFullRoute { get; set; }

        public string RequestStatus { get; set; }

        public DateTime RequestToDate { get; set; }
    }
}