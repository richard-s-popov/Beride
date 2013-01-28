using System;
using TransportSystem.Domain;

namespace TransportSystem.Logics.Interfaces.Membership
{
    /// <summary>
    /// Interface for managing user's database
    /// </summary>
    public interface IUsersService : IDisposable
    {
        /// <summary>
        /// Inserts new entity into database
        /// </summary>
        /// <param name="entity"></param>
        void Insert(User entity);

        /// <summary>
        /// Get entity user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User GetUserByLogin(string email);

        /// <summary>
        /// Check email in db
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool EmailIsExist(string email);

        /// <summary>
        /// Check pnone number in db
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        bool PhoneIsExist(string phone);
    }
}
