using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Logics.Interfaces;
using TransportSystem.Logics.Interfaces.Trips;
using TransportSystem.Domain;

namespace TransportSystem.Logics.Impl.Trips
{
    public class TripsService : ITripsService
    {
        public Entities db = null;

        public TripsService()
        {
            db = new Entities();
        }
   
        public Trip GetById(long id)
        {
            return db.Trip.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Trip entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(Trip entity)
        {
            db.AddToTrip(entity);
        }

        public void AddRoute(TripRoute entity)
        {
            db.AddToTripRoute(entity);
        }

        public TripRoute GetRouteById(long id)
        {
            return db.TripRoute
                .Include("Trip")
                .FirstOrDefault(x => x.Id == id);
        }

        public TripDate GetTripDateById(long id)
        {
            return db.TripDate.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Trip> GetTripsByUserId(int userId)
        {
            return db.Trip.Where(x => x.OwnerId == userId);
        }

        public IEnumerable<GetActiveTripsByUserAndRoute_Result> GetActiveTripsByUserAndRoute(int userId, string startPoint, string endPoint, DateTime date, int type)
        {
            return db.GetActiveTripsByUserAndRoute(date, userId, startPoint, endPoint, type);
        }

        public IEnumerable<GetTrips_Result> GetTrips(string startPointGid, string endPointGid, DateTime dateAt, DateTime dateTo, int tripType, int tripStatus)
        {
            return db.GetTrips(startPointGid, endPointGid, dateAt, dateTo, tripType, tripStatus);
        }

        public IEnumerable<GetTripsByUser_Result> GetTripsByUser(int userId)
        {
            return db.GetTripsByUser(userId);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
