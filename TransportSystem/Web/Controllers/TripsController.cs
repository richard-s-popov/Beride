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
using TransportSystem.Area.Web.Models;
using TransportSystem.Area.Web.Models.Trips;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.Trip;
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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateTrip()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveTrip(TripModel model, string points)
        {
            //var currentUser = _usersService.GetUserByLogin(System.Web.HttpContext.Current.User.Identity.Name);
            var trip = new Trips();

            trip.CreatorId = 1;//currentUser.Id;
            trip.OwnerId = 1;//currentUser.Id;

            if (model.IsDriver)
            {
                if (model.FreeDriver != null && model.FreeDriver.Value)
                {
                    trip.TripType = TripType.FreeDriver;
                }
                else
                {
                    trip.TripType = TripType.Driver;
                }
            }
            else
            {
                trip.TripType = TripType.Passenger;
            }


            // Дисериализуем строку с массивом точек
            var js = new JavaScriptSerializer();
            var deserializedPoints = (object[])js.DeserializeObject(points);
            var myPoints = new List<SJSonModel>();

            if (deserializedPoints != null)
            {
                // получаем массив точек
                foreach (Dictionary<string, object> newFeature in deserializedPoints)
                {
                    myPoints.Add(new SJSonModel(newFeature));
                }
            }

            // генерируем всевозможные варианты маршрутов по этому пути точек
            for (int i = 0; i < myPoints.Count - 1; i++)
            {
                for (int j = i + 1; j < myPoints.Count; j++)
                {
                    var route = new TripRoutes();
                    
                    route.StartPointGid = myPoints[i].Gid;
                    route.StartPointFullName = myPoints[i].FullName;
                    route.StartPointShortName = myPoints[i].ShortName;

                    route.EndPointGid = myPoints[j].Gid;
                    route.EndPointFullName = myPoints[j].FullName;
                    route.EndPointShortName = myPoints[j].ShortName;

                    trip.TripRoutes.Add(route);
                }
            }

            trip.Date = DateTime.Now;
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

        public ActionResult GetTrips(string startPointGid, string endPointGid, DateTime dateAt, DateTime? dateTo)
        {
            var trips = _tripsService.GetTrips(startPointGid, endPointGid, dateAt, dateAt.AddDays(7), 1, 1);

            var model = new TripsListModel();

            foreach (var trip in trips)
            {
                var m = new TripModel
                    {
                        StartPointFullName = trip.StartPointFullName, 
                        EndPointFullName = trip.EndPointFullName
                    };
                model.trips.Insert(model.trips.Count, m);
            }

            return this.View("TripsList", model);
        }

        protected override void Dispose(bool disposing)
        {
            _tripsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
