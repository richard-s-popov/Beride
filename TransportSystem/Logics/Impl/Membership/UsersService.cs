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
            // страшный костыль
            // алиасы для России
            login = login.Replace("+78", "78");
            login = login.Replace("+79", "79");
            login = login.First() == '8' ? "7" + login.Substring(1) : login;

            // алиас для Казахстана
            login = login.Replace("+77", "77");

            return db.User.FirstOrDefault(x => x.Email == login || x.Phone == login);
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
