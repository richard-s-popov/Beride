using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Area.Web.Infrastructure.Attributes;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.Trips;

namespace TransportSystem.Area.Web.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ITripsService _tripsService;

        private readonly IUsersService _usersService;

        public RequestsController(
            ITripsService tripsService,
            IUsersService usersService)
        {
            _tripsService = tripsService;
            _usersService = usersService;
        }

        [Secure]
        [HttpPost]
        public ActionResult SendRequest(int tripId)
        {
            

            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            _usersService.Dispose();
            _tripsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
