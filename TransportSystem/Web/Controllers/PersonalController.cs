using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Area.Web.Infrastructure.Attributes;
using TransportSystem.Area.Web.Models.Personal;
using TransportSystem.Area.Web.Models.Requests;
using TransportSystem.Area.Web.Models.Trips;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.Trips;

namespace TransportSystem.Area.Web.Controllers
{
    public class PersonalController : Controller
    {
        private readonly IUsersService _usersService;

        private readonly IRequestsService _requestsService;

        private readonly ITripsService _tripsService;

        public PersonalController(
            ITripsService tripsService,
            IUsersService usersService,
            IRequestsService requestsService)
        {
            _tripsService = tripsService;
            _usersService = usersService;
            _requestsService = requestsService;
        }

        [Secure]
        public ActionResult MyTripsCabinet()
        {
            var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var userTrips = _tripsService.GetTripsByUser(currentUser.Id).ToList();

            var userRequests = new List<GetRequestsByUserAndTrip_Result>();

            if (userTrips.Any())
            {
                userRequests = _requestsService.GetRequestsByUserAndTrip(currentUser.Id, userTrips.First().TripId,
                                                                         userTrips.First().TripType).ToList();
            }

            var model = new MyTripsCabinetModel
                {
                    Trips = userTrips.GroupBy(x => x.TripId).Select(group => new TripModel
                        {
                            TripId = group.Key,
                            MainRouteShortString = group.First().MainRouteShortStr.Replace(";", " → "),
                            StartDateAt = group.Min(d => d.Date),
                            StartDateTo = group.Max(d => d.Date)
                        }).ToList(),
                    Requests = userRequests.Select(entity => new RequestModel
                        {
                            UserFullName = string.Format("{0} {1}", entity.OwnerFisrtName, entity.OwnerLastName),
                            UserFullRoute = entity.MainRouteShortStr.Replace(";", " → "),
                            RequestStatus = GetRequestStatus(entity.StatusRequestId),
                            RequestToDate = entity.RequestToDate,
                            Cost = entity.Cost,
                            RequestType = entity.TripType,
                            ToMe = entity.InitiatorId != currentUser.Id
                        }).ToList()
                };

            return PartialView(model);
        }

        private string GetRequestStatus(int statusId)
        {
            switch (statusId)
            {
                case 1:
                    return "Ожидает";
                case 2:
                    return "Отклонена";
                case 3:
                    return "Одобрена";
                case 4:
                    return "Отменена";
                default:
                    return string.Empty;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _usersService.Dispose();
            _tripsService.Dispose();
            _requestsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
