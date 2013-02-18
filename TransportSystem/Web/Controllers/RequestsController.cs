using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Area.Web.Infrastructure.Attributes;
using TransportSystem.Area.Web.Models.Requests;
using TransportSystem.Area.Web.Models.Trips;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.SMS;
using TransportSystem.Logics.Interfaces.Trips;
using TripType = TransportSystem.Domain.Enums.TripType;

namespace TransportSystem.Area.Web.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ITripsService _tripsService;

        private readonly IUsersService _usersService;

        private readonly IRequestsService _requestsService;

        private readonly ISMS _smsService;

        public RequestsController(
            ITripsService tripsService,
            IUsersService usersService,
            IRequestsService requestsService,
            ISMS smsService)
        {
            _tripsService = tripsService;
            _usersService = usersService;
            _requestsService = requestsService;
            _smsService = smsService;
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

            var thisRequest = _requestsService.GetRequest(currentUser.Id, routeId, tripDateId);
            if (thisRequest != null)
            {
                ViewBag.Error = "Заявка на эту поездку уже отправлена";
                return PartialView("Error");
            }

            var searchingType = hisTrip.TripType == TripType.Driver ? TripType.Passenger : TripType.Driver;
            var myTrips = _tripsService.GetActiveTripsByUserAndRoute(currentUser.Id, hisRoute.StartPointGid, hisRoute.EndPointGid, DateTime.Today, searchingType).ToList();

            string message;
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
                var myRoute =
                    myTrip.TripRoute.FirstOrDefault(
                        x => x.StartPointGid == hisRoute.StartPointGid && x.EndPointGid == hisRoute.EndPointGid);

                if (myTrip.TripType == TripType.Passenger)
                {
                    if (myTrip.TripDate.FirstOrDefault(d => d.Date == hisTripDate.Date) == null)
                    {
                        myTrip.TripDate.Add(new TripDate
                            {
                                Date = hisTripDate.Date,
                                IsDeleted = false
                            });
                    }
                }
                
                var request = new Request
                {
                    DriverTripId = myTrip.TripType == TripType.Driver ? myTrip.Id : hisTrip.Id,
                    PassengerTripId = myTrip.TripType == TripType.Passenger ? myTrip.Id : hisTrip.Id,
                    OwnerRouteId = hisRoute.Id,
                    OwnerTripDateId = hisTripDate.Id,
                    InitiatorId = currentUser.Id,
                    Cost = myTrip.TripType == TripType.Driver ? myRoute.Cost : 0,
                    StatusRequestId = 1,
                    CreateDate = DateTime.Now,
                    RequestToDate = hisTrip.TripType == TripType.Driver ? myTrip.TripDate.First().Date : hisTripDate.Date
                };

                _requestsService.Insert(request);
                _requestsService.SaveChanges();
                _tripsService.Save();

                message = string.Format(
                    "Вам пришла новая заявка:\n{0}-{1}\n{2}\n{3} {4}.\nВы можете Принять или Отклонить в личном кабинете. www.beride.ru", myRoute.StartPointShortName, myRoute.EndPointShortName, hisTripDate.Date.ToShortDateString(), currentUser.FirstName, currentUser.LastName);

                _smsService.SendMessage(hisTrip.User.Phone, message);

                return PartialView("YourRequestSent");
            }

            // Создаем новую поездку со стороны пользователя, если у него нет активных поездок
            var myNewTrip = new Trip
                {
                    OwnerId = currentUser.Id,
                    Seats = 1,
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
                    EndPointShortName = hisRoute.EndPointShortName,
                    Cost = 0 // Надо сделать добавление стоимости из расчета автоматом
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
                RequestToDate = hisTrip.TripType == TripType.Driver ? myNewTrip.TripDate.First().Date : hisTripDate.Date,
                Cost = 0
            };

            _requestsService.Insert(entity);
            _requestsService.SaveChanges();

            message = string.Format(
                "Вам пришла новая заявка:\n{0}-{1}\n{2}\n{3} {4}.\nВы можете Принять или Отклонить в личном кабинете. www.beride.ru", 
                myNewTrip.TripRoute.First().StartPointShortName, 
                myNewTrip.TripRoute.First().EndPointShortName, 
                hisTripDate.Date.ToShortDateString(), 
                currentUser.FirstName, 
                currentUser.LastName);

            _smsService.SendMessage(hisTrip.User.Phone, message);

            return PartialView("YourTripCreated");
        }

        [Secure]
        public ActionResult GetRequestsByTrip(long tripId)
        {
            var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var trip = _tripsService.GetById(tripId);

            var userRequests = _requestsService.GetRequestsByUserAndTrip(currentUser.Id, trip.Id, trip.TripType).ToList();

            var model = new RequestsListModel
                {
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

            return PartialView("RequestsListByTrip", model);
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
