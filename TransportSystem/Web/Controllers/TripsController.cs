using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using TransportSystem.Area.Web.Infrastructure.Attributes;
using TransportSystem.Area.Web.Models.Trips;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.Trips;
using TripType = TransportSystem.Domain.Enums.TripType;

namespace TransportSystem.Area.Web.Controllers
{
    public class TripsController : Controller
    {
        private readonly ITripsService _tripsService;

        private readonly IUsersService _usersService;

        public TripsController(
            ITripsService tripsService,
            IUsersService usersService)
        {
            _tripsService = tripsService;
            _usersService = usersService;
        }

        public ActionResult CreateTrip()
        {
            return View();
        }

        [Secure]
        [HttpPost]
        public ActionResult SaveTrip(TripModel model, string points, DateTime dateAt, DateTime? dateTo)
        {
            var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var trip = new Trip
                {
                    OwnerId = currentUser.Id
                };

            // Дисериализуем строку с массивом точек
            var js = new JavaScriptSerializer();
            var deserializedPoints = (object[])js.DeserializeObject(points);
            var myPoints = new List<SJSonModel>();

            if (deserializedPoints != null)
            {
                // получаем массив точек
                myPoints.AddRange(from Dictionary<string, object> newFeature in deserializedPoints select new SJSonModel(newFeature));

                var mainRouteStr = new List<string>();
                var mainRouteShortStr = new List<string>();
                
                foreach (var point in myPoints)
                {
                    mainRouteStr.Add(point.FullName);
                    mainRouteShortStr.Add(point.ShortName);
                }

                trip.MainRouteStr = string.Join(";", mainRouteStr);
                trip.MainRouteShortStr = string.Join(";", mainRouteShortStr);
            }

            if (model.IsDriver)
            {
                trip.TripType = TripType.Driver;

                // генерируем всевозможные варианты маршрутов по этому пути точек
                for (var i = 0; i < myPoints.Count - 1; i++)
                {
                    for (var j = i + 1; j < myPoints.Count; j++)
                    {
                        var route = new TripRoute
                        {
                            StartPointGid = myPoints[i].Gid,
                            StartPointFullName = myPoints[i].FullName,
                            StartPointShortName = myPoints[i].ShortName,
                            EndPointGid = myPoints[j].Gid,
                            EndPointFullName = myPoints[j].FullName,
                            EndPointShortName = myPoints[j].ShortName
                        };

                        trip.TripRoute.Add(route);    
                    }
                }
            }
            else
            {
                trip.TripType = TripType.Passenger;

                // генерируем маршрут только из точки А в точку В
                trip.TripRoute.Add(new TripRoute
                    {
                        StartPointGid = myPoints.First().Gid,
                        StartPointFullName = myPoints.First().FullName,
                        StartPointShortName = myPoints.First().ShortName,
                        EndPointGid = myPoints.Last().Gid,
                        EndPointFullName = myPoints.Last().FullName,
                        EndPointShortName = myPoints.Last().ShortName
                    });
            }

            if (dateTo == null)
            {
                trip.TripDate.Add(new TripDate
                {
                    Date = dateAt,
                    IsDeleted = false
                });
            }
            else
            {
                for (var date = dateAt; date <= dateTo.Value; date = date.AddDays(1))
                {
                    trip.TripDate.Add(new TripDate
                    {
                        Date = date,
                        IsDeleted = false
                    });
                }
            }

            trip.Seats = model.SeatsNumber;
            trip.FreeSeats = model.SeatsNumber;
            trip.TripStatus = 1;
            trip.IsDeleted = false;

            _tripsService.Insert(trip);
            _tripsService.Save();

            return this.RedirectToAction("Index", "Home");
        }

        public JsonResult GetPlaces(string geoName)
        {
            var cache = HttpRuntime.Cache;
            string googleResponse;

            lock (cache)
            {
                googleResponse = (string)cache[geoName];

                if (googleResponse != null)
                {
                    return Json(googleResponse, "text/plain", JsonRequestBehavior.AllowGet);
                }
            }

            var myHttpWebRequest = (HttpWebRequest)WebRequest
                .Create(string.Format("https://maps.googleapis.com/maps/api/place/autocomplete/{0}?input={1}&types={2}&language={3}&sensor=false&key={4}", "json", geoName, "geocode", "ru", "AIzaSyCQIeGPtr9iyky2M-MCxHDGyPqn6WrtdVM"));
            
            var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            
            var myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream(), Encoding.UTF8);

            googleResponse = myStreamReader.ReadToEnd();
            dynamic json = JValue.Parse(googleResponse); 

            cache.Insert(geoName, googleResponse, null, DateTime.Now.AddHours(24), Cache.NoSlidingExpiration);

            return Json(googleResponse, "text/plain", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTrips(string startPointGid, string endPointGid, DateTime dateAt, DateTime? dateTo, string search)
        {
            if (dateTo == null)
            {
                dateTo = dateAt;
            }

            var trips = _tripsService.GetTrips(startPointGid, endPointGid, dateAt, dateTo.Value, search == "driver" ? 2 : 1, 1).OrderBy(x => x.Date).ToList();

            var model = new TripsListModel();

            if (trips.Any())
            {
                foreach (var trip in trips)
                {
                    var m = new TripModel
                    {
                        TripId = trip.Id,
                        RouteId = trip.RouteId,
                        TripDateId = trip.TripDateId,
                        StartPointFullName = trip.StartPointFullName,
                        StartPointShortName = trip.StartPointShortName,
                        EndPointFullName = trip.EndPointFullName,
                        EndPointShortName = trip.EndPointShortName,
                        MainRouteStr = trip.MainRouteStr.Split(';'),
                        MainRouteShortStr = trip.MainRouteShortStr.Split(';'),
                        IsDriver = trip.TripType == TripType.Driver,
                        Date = trip.Date,
                        SeatsNumber = trip.Seats
                    };
                    model.trips.Add(m);
                }
            }

            return this.View("TripsList", model);
        }

        protected override void Dispose(bool disposing)
        {
            _usersService.Dispose();
            _tripsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
