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
            throw new NotImplementedException();
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

        public IEnumerable<GetTrips_Result> GetTrips(string startPointGid, string endPointGid, DateTime dateAt, DateTime dateTo, int tripType, int tripStatus)
        {
            return db.GetTrips(startPointGid, endPointGid, dateAt, dateTo, tripType, tripStatus);
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
