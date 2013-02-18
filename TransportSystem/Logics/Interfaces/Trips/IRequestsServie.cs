using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Domain;

namespace TransportSystem.Logics.Interfaces.Trips
{
    /// <summary>
    /// Интерфейс для сервиса заявок
    /// </summary>
    public interface IRequestsService : ICrudService<Request>, IDisposable
    {
        /// <summary>
        /// Сохранить изменения
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Возвращает заявки по пользователю для поездки
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tripId"></param>
        /// <param name="tripType"></param>
        /// <returns></returns>
        IEnumerable<GetRequestsByUserAndTrip_Result> GetRequestsByUserAndTrip(int userId, int tripId, int tripType);

        /// <summary>
        /// Возвращает актуальные заявки по точкам маршрута
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="startPointGid"></param>
        /// <param name="endPointGid"></param>
        /// <returns></returns>
        IEnumerable<GetActualRequests_Result> GetActualRequests(int userId, string startPointGid, string endPointGid);

        /// <summary>
        /// Возвращает заявку по маршруту и дате
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="routeId"></param>
        /// <param name="tripDateId"></param>
        /// <returns></returns>
        Request GetRequest(int userId, long routeId, long tripDateId);
    }
}
