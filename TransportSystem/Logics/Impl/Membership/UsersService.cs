using System;
using System.Linq;
using TransportSystem.Domain;
using TransportSystem.Logics.Interfaces.Membership;

namespace TransportSystem.Logics.Impl.Membership
{
    public class UsersService : IUsersService
    {
        public Entities db = null;

        public void Insert(Users entity)
        {
            db.AddToUsers(entity);
            db.SaveChanges();
        }

        public Users GetUserByLogin(string login)
        {
            return db.Users.First(x => x.Email == login || x.Phone == login);
        }

        public bool EmailIsExist(string email)
        {
            throw new NotImplementedException();
        }

        public bool PhoneIsExist(string phone)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
