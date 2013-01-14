using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Logics.Interfaces;
using TransportSystem.Logics.Interfaces.Trip;
using TransportSystem.Domain;

namespace TransportSystem.Logics.Impl.Trip
{
    public class TripsService : ITripsService
    {
        public Entities db = null;

        public TripsService()
        {
            db = new Entities();
        }
   
        public Trips GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Trips entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(Trips entity)
        {
            db.AddToTrips(entity);
        }

        public void AddRoute(TripRoutes entity)
        {
            db.AddToTripRoutes(entity);
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
