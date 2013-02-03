using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Area.Web.Infrastructure.Attributes;
using TransportSystem.Area.Web.Models.Trips;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.Trips;
using TripType = TransportSystem.Domain.Enums.TripType;

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
        public ActionResult SendRequest(long tripDateId, long routeId)
        {
            var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var tripDate = _tripsService.GetTripDateById(tripDateId);
            var someoneTrip = tripDate.Trip;
            var someoneRoute = someoneTrip.TripRoute.FirstOrDefault(x => x.Id == routeId);

            if (someoneRoute == null)
            {
                ViewBag.Error = "Данного маршрута не существует";
                return PartialView("Error");
            }

            if (tripDate.Date < DateTime.Today)
            {
                ViewBag.Error = "Вы пытаетесь отправить заявку на прошедшую поездку";
                return PartialView("Error");
            }

            var searchingType = someoneTrip.TripType == TripType.Driver ? TripType.Passenger : TripType.Driver;
            var myTrips = _tripsService.GetActiveTripsByUser(currentUser.Id, DateTime.Now, searchingType).ToList();

            if (myTrips.Any())
            {
                var model = new TripsListModel
                    {
                        trips = myTrips.GroupBy(trip => trip.TripId).Select(group => new TripModel
                        {
                            MainRouteShortStr = group.First().MainRouteShortStr.Split(';'),
                            MainRouteStr = group.First().MainRouteStr.Split(';'),
                            StartDateAt = group.Min(x => x.Date),
                            StartDateTo = group.Max(x => x.Date)
                        }).ToList()
                    };

                return PartialView("MyTripsList", model);
            }

            // Создаем новую поездку со стороны пользователя, если у него нет активных поездок
            var myNewTrip = new Trip
                {
                    OwnerId = currentUser.Id,
                    TripStatus = 1,
                    TripType = searchingType,
                    MainRouteStr = string.Format("{0};{1}", someoneRoute.StartPointFullName, someoneRoute.EndPointFullName),
                    MainRouteShortStr = string.Format("{0};{1}", someoneRoute.StartPointShortName, someoneRoute.EndPointShortName),
                    IsDeleted = false
                };

            myNewTrip.TripRoute.Add(new TripRoute
                {
                    StartPointGid = someoneRoute.StartPointGid,
                    StartPointFullName = someoneRoute.StartPointFullName,
                    StartPointShortName = someoneRoute.StartPointShortName,
                    EndPointGid = someoneRoute.EndPointGid,
                    EndPointFullName = someoneRoute.EndPointFullName,
                    EndPointShortName = someoneRoute.EndPointShortName
                });

            myNewTrip.TripDate.Add(new TripDate
                {
                    Date = tripDate.Date,
                    IsDeleted = false
                });

            _tripsService.Insert(myNewTrip);
            _tripsService.Save();

            return PartialView("YourTripsCreated");
        }

        protected override void Dispose(bool disposing)
        {
            _usersService.Dispose();
            _tripsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
