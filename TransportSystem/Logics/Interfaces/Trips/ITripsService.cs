using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Domain;

namespace TransportSystem.Logics.Interfaces.Trips
{
    /// <summary>
    /// Интерфейс для "поездок"
    /// </summary>
    public interface ITripsService : ICrudService<Trip>, IDisposable
    {
        void AddRoute(TripRoute entity);

        TripRoute GetRouteById(long id);

        TripDate GetTripDateById(long id);

        IEnumerable<Trip> GetTripsByUserId(int userId);

        IEnumerable<GetActiveTripsByUserAndRoute_Result> GetActiveTripsByUserAndRoute(int userId, string startPoint, string endPoint, DateTime date, int type);
            
        IEnumerable<GetTrips_Result> GetTrips(string startPointGid, string endPointGid, DateTime dateAt, DateTime dateTo, int tripType, int tripStatus);

        IEnumerable<GetTripsByUser_Result> GetTripsByUser(int userId); 

        void Save();
    }
}
