using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Trips;

namespace TransportSystem.Logics.Impl.Trips
{
    public class RequestsService : IRequestsService
    {
        public Entities db = null;

        public RequestsService()
        {
            db = new Entities();
        }

        public Request GetById(long id)
        {
            return db.Request.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Request entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(Request entity)
        {
            db.AddToRequest(entity);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }

        public IEnumerable<GetRequestsByUserAndTrip_Result> GetRequestsByUserAndTrip(int userId, int tripId, int tripType)
        {
            return db.GetRequestsByUserAndTrip(userId, tripId, tripType);
        }

        public IEnumerable<GetActualRequests_Result> GetActualRequests(int userId, string startPointGid, string endPointGid)
        {
            return db.GetActualRequests(userId, startPointGid, endPointGid, DateTime.Today);
        }

        public Request GetRequest(int userId, long routeId, long tripDateId)
        {
            return
                db.Request.FirstOrDefault(
                    x => x.OwnerRouteId == routeId && x.OwnerTripDateId == tripDateId && x.InitiatorId == userId);
        }
    }
}
