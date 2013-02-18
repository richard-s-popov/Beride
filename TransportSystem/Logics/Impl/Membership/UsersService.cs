using System;
using System.Linq;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;

namespace TransportSystem.Logics.Impl.Membership
{
    public class UsersService : IUsersService
    {
        public Entities db = null;

        public UsersService()
        {
            db = new Entities();
        }

        public void Insert(User entity)
        {
            db.AddToUser(entity);
            db.SaveChanges();
        }

        public User GetUserByLogin(string login)
        {
            return db.User.FirstOrDefault(x => x.Email == login || x.Phone == login && x.IsConfirmed);
        }

        public bool EmailIsExist(string email)
        {
            return db.User.FirstOrDefault(x => x.Email == email) != null;
        }

        public bool PhoneIsExist(string phone)
        {
            return db.User.FirstOrDefault(x => x.Phone == phone) != null;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
