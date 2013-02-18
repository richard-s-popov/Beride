using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using TransportSystem.Domain;
using TransportSystem.Logics.Infrastructure.Extensions;
using TransportSystem.Logics.Interfaces.Membership;

namespace TransportSystem.Logics.Impl.Membership
{
    class MembershipService : IMembershipService
    {
        public Entities db = null;

        public MembershipService()
        {
            db = new Entities();
        }

        public bool AuthorizeUser(string login, string password)
        {
            // страшный костыль
            // алиасы для России
            login = login.Replace("+78", "78");
            login = login.Replace("+79", "79");
            login = login.First() == '8' ? "7" + login.Substring(1) : login;
            
            // алиас для Казахстана
            login = login.Replace("+77", "77");

            password = password.Md5();

            return db.User.FirstOrDefault(u => (u.Email == login || u.Phone == login) && u.Password == password) != null;
        }

        public void LoginUser(HttpContext currentHttpContext, string login, string password, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "login");

            FormsAuthentication.SetAuthCookie(login, createPersistentCookie);
        }

        public void LogoutCurrentUser(HttpContext currentHttpContext)
        {
            FormsAuthentication.SignOut();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
