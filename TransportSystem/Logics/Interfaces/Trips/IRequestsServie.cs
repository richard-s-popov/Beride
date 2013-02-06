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
    }
}
