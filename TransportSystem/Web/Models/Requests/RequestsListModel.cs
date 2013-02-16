using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportSystem.Area.Web.Models.Requests
{
    public class RequestsListModel
    {
        public IList<RequestModel> Requests { get; set; } 
    }
}