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

        private readonly IRequestsService _requestsService;

        public RequestsController(
            ITripsService tripsService,
            IUsersService usersService,
            IRequestsService requestsService)
        {
            _tripsService = tripsService;
            _usersService = usersService;
            _requestsService = requestsService;
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
            var myTrips = _tripsService.GetActiveTripsByUser(currentUser.Id, DateTime.Today, searchingType).ToList();

            if (myTrips.Any())
            {
                var model = new TripsListModel
                    {
                        trips = myTrips.GroupBy(trip => trip.TripId).Select(group => new TripModel
                        {
                            TripId = group.Key,
                            MainRouteShortStr = group.First().MainRouteShortStr.Split(';'),
                            MainRouteStr = group.First().MainRouteStr.Split(';'),
                            StartDateAt = group.Min(x => x.Date),
                            StartDateTo = group.Max(x => x.Date),
                            StartPointShortName = someoneRoute.StartPointShortName,
                            EndPointShortName = someoneRoute.EndPointShortName
                        }).ToList()
                    };

                ViewBag.OwnerTripRouteId = routeId;
                ViewBag.OwnerTripDateId = tripDateId;

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

            var entity = new Request
            {
                DriverTripId = someoneTrip.TripType == TripType.Driver ? someoneTrip.Id : myNewTrip.Id,
                PassengerTripId = someoneTrip.TripType == TripType.Driver ? myNewTrip.Id : someoneTrip.Id,
                OwnerRouteId = someoneRoute.Id,
                OwnerTripDateId = tripDate.Id,
                InitiatorId = currentUser.Id,
                StatusRequestId = 1,
                CreateDate = DateTime.Now
            };

            _requestsService.Insert(entity);
            _requestsService.SaveChanges();

            return PartialView("YourTripCreated");
        }

        [Secure]
        [HttpPost]
        public ActionResult SelectTrip(int myTripId, long ownerTripDateId, long ownerTripRouteId)
        {
            var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var ownerTripDate = _tripsService.GetTripDateById(ownerTripDateId);
            var ownerTrip = ownerTripDate.Trip;
            var ownerRoute = ownerTrip.TripRoute.FirstOrDefault(x => x.Id == ownerTripRouteId);
            var myTrip = _tripsService.GetById(myTripId);

            if (ownerTrip != null && ownerRoute != null && myTrip != null)
            {
                myTrip.TripDate.Add(new TripDate
                    {
                        Date = ownerTripDate.Date,
                        IsDeleted = false
                    });

                var entity = new Request
                    {
                        DriverTripId = ownerTrip.TripType == TripType.Driver ? ownerTrip.Id : myTripId,
                        PassengerTripId = ownerTrip.TripType == TripType.Driver ? myTripId : ownerTrip.Id,
                        OwnerRouteId = ownerRoute.Id,
                        OwnerTripDateId = ownerTripDate.Id,
                        InitiatorId = currentUser.Id,
                        StatusRequestId = 1,
                        CreateDate = DateTime.Now
                    };

                _requestsService.Insert(entity);
                _requestsService.SaveChanges();
            }

            return PartialView("YourRequestSent");
        }

        [Secure]
        [HttpPost]
        public ActionResult CreateNewTrip(long ownerTripRouteId, long ownerTripDateId)
        {
            var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var ownerTripDate = _tripsService.GetTripDateById(ownerTripDateId);
            var ownerTrip = ownerTripDate.Trip;
            var ownerRoute = ownerTrip.TripRoute.FirstOrDefault(x => x.Id == ownerTripRouteId);

            if (ownerTrip != null && ownerRoute != null)
            {
                var newTrip = new Trip
                    {
                        OwnerId = currentUser.Id,
                        TripStatus = 1,
                        TripType = ownerTrip.TripType == TripType.Driver ? TripType.Passenger : TripType.Driver,
                        MainRouteStr = string.Format("{0};{1}", ownerRoute.StartPointFullName, ownerRoute.EndPointFullName),
                        MainRouteShortStr = string.Format("{0};{1}", ownerRoute.StartPointShortName, ownerRoute.EndPointShortName),
                        IsDeleted = false
                    };

                newTrip.TripRoute.Add(new TripRoute
                {
                    StartPointGid = ownerRoute.StartPointGid,
                    StartPointFullName = ownerRoute.StartPointFullName,
                    StartPointShortName = ownerRoute.StartPointShortName,
                    EndPointGid = ownerRoute.EndPointGid,
                    EndPointFullName = ownerRoute.EndPointFullName,
                    EndPointShortName = ownerRoute.EndPointShortName
                });

                newTrip.TripDate.Add(new TripDate
                {
                    Date = ownerTripDate.Date,
                    IsDeleted = false
                });

                _tripsService.Insert(newTrip);
                _tripsService.Save();

                var entity = new Request
                {
                    DriverTripId = ownerTrip.TripType == TripType.Driver ? ownerTrip.Id : newTrip.Id,
                    PassengerTripId = ownerTrip.TripType == TripType.Driver ? newTrip.Id : ownerTrip.Id,
                    OwnerRouteId = ownerRoute.Id,
                    OwnerTripDateId = ownerTripDate.Id,
                    InitiatorId = currentUser.Id,
                    StatusRequestId = 1,
                    CreateDate = DateTime.Now
                };

                _requestsService.Insert(entity);
                _requestsService.SaveChanges();
            }

            return PartialView("YourRequestSent");
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
