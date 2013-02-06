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
    }
}
