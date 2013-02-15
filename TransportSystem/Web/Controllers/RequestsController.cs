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
            var hisTripDate = _tripsService.GetTripDateById(tripDateId);
            var hisTrip = hisTripDate.Trip;
            var hisRoute = hisTrip.TripRoute.FirstOrDefault(x => x.Id == routeId);

            if (hisTrip.OwnerId == currentUser.Id)
            {
                ViewBag.Error = "Вы пытаетесь отправить заявку самому себе";
                return PartialView("Error");
            }

            if (hisRoute == null)
            {
                ViewBag.Error = "Данного маршрута не существует";
                return PartialView("Error");
            }

            if (hisTripDate.Date < DateTime.Today)
            {
                ViewBag.Error = "Вы пытаетесь отправить заявку на прошедшую поездку";
                return PartialView("Error");
            }

            var searchingType = hisTrip.TripType == TripType.Driver ? TripType.Passenger : TripType.Driver;
            var myTrips = _tripsService.GetActiveTripsByUserAndRoute(currentUser.Id, hisRoute.StartPointGid, hisRoute.EndPointGid, DateTime.Today, searchingType).ToList();

            if (myTrips.Any())
            {
                // До лучших времен. Выбор к какой поездке принадлежит заявка
                /* var model = new TripsListModel
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

                return PartialView("MyTripsList", model); */

                var myTrip = _tripsService.GetById(myTrips.First().TripId);

                if (myTrip.TripType == TripType.Passenger)
                {
                    myTrip.TripDate.Add(new TripDate
                    {
                        Date = hisTripDate.Date,
                        IsDeleted = false
                    });
                }
                
                var request = new Request
                {
                    DriverTripId = myTrip.TripType == TripType.Driver ? myTrip.Id : hisTrip.Id,
                    PassengerTripId = myTrip.TripType == TripType.Passenger ? myTrip.Id : hisTrip.Id,
                    OwnerRouteId = hisRoute.Id,
                    OwnerTripDateId = hisTripDate.Id,
                    InitiatorId = currentUser.Id,
                    StatusRequestId = 1,
                    CreateDate = DateTime.Now,
                    RequestToDate = hisTrip.TripType == TripType.Driver ? myTrip.TripDate.First().Date : hisTripDate.Date
                };

                _requestsService.Insert(request);
                _requestsService.SaveChanges();
                _tripsService.Save();

                return PartialView("YourRequestSent");
            }

            // Создаем новую поездку со стороны пользователя, если у него нет активных поездок
            var myNewTrip = new Trip
                {
                    OwnerId = currentUser.Id,
                    TripStatus = 1,
                    TripType = searchingType,
                    MainRouteStr = string.Format("{0};{1}", hisRoute.StartPointFullName, hisRoute.EndPointFullName),
                    MainRouteShortStr = string.Format("{0};{1}", hisRoute.StartPointShortName, hisRoute.EndPointShortName),
                    IsDeleted = false
                };

            myNewTrip.TripRoute.Add(new TripRoute
                {
                    StartPointGid = hisRoute.StartPointGid,
                    StartPointFullName = hisRoute.StartPointFullName,
                    StartPointShortName = hisRoute.StartPointShortName,
                    EndPointGid = hisRoute.EndPointGid,
                    EndPointFullName = hisRoute.EndPointFullName,
                    EndPointShortName = hisRoute.EndPointShortName
                });

            myNewTrip.TripDate.Add(new TripDate
                {
                    Date = hisTripDate.Date,
                    IsDeleted = false
                });

            _tripsService.Insert(myNewTrip);
            _tripsService.Save();

            var entity = new Request
            {
                DriverTripId = hisTrip.TripType == TripType.Driver ? hisTrip.Id : myNewTrip.Id,
                PassengerTripId = hisTrip.TripType == TripType.Driver ? myNewTrip.Id : hisTrip.Id,
                OwnerRouteId = hisRoute.Id,
                OwnerTripDateId = hisTripDate.Id,
                InitiatorId = currentUser.Id,
                StatusRequestId = 1,
                CreateDate = DateTime.Now,
                RequestToDate = hisTrip.TripType == TripType.Driver ? myNewTrip.TripDate.First().Date : hisTripDate.Date
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
                        CreateDate = DateTime.Now,
                        RequestToDate = ownerTripDate.Date
                    };

                _requestsService.Insert(entity);
                _requestsService.SaveChanges();
                _tripsService.Save();
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
                    CreateDate = DateTime.Now,
                    RequestToDate = ownerTripDate.Date
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
