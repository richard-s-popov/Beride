using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransportSystem.Domain;

namespace TransportSystem.Logics.Interfaces.SMS
{
    /// <summary>
    /// Интерфейс для сервиса СМС сообщений
    /// </summary>
    public interface ISMS : IDisposable
    {
        /// <summary>
        /// Отправка сообщения на номер
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        void SendMessage(string number, string message);
    }
}
