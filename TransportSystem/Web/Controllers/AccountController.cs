using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Logics.Interfaces.Membership;

namespace TransportSystem.Area.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersService _usersService;

        public AccountController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginForm()
        {
            return this.View();
        }

        public ActionResult Login()
        {
            return new EmptyResult();
        }

        public ActionResult Register()
        {
            return this.View();
        }

        public ActionResult RegistrationFinish()
        {
            return new EmptyResult();
        }
    }
}
