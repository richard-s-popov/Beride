using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Domain;
using TransportSystem.Domain.Enums;

namespace TransportSystem.Logics.Interfaces.Trip
{
    /// <summary>
    /// Интерфейс для "поездок"
    /// </summary>
    public interface ITripsService : ICrudService<Trips>, IDisposable
    {
        void AddRoute(TripRoutes entity);

        IEnumerable<GetTrips_Result> GetTrips(string startPointGid, string endPointGid, DateTime dateAt, DateTime dateTo, int tripType, int tripStatus);

        void Save();
    }
}
