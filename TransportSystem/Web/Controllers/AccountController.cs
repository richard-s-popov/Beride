using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Area.Web.Models.Account;
using TransportSystem.Domain;
using TransportSystem.Logics.Infrastructure.Extensions;
using TransportSystem.Logics.Interfaces.Membership;

namespace TransportSystem.Area.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersService _usersService;

        private readonly IMembershipService _membershipService;

        public AccountController(
            IUsersService usersService,
            IMembershipService membershipService)
        {
            _usersService = usersService;
            _membershipService = membershipService;
        }

        [HttpPost]
        public JsonResult Login(LogOnModelPoco model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_membershipService.AuthorizeUser(model.Login, model.Password))
                {
                    _membershipService.LoginUser(null, model.Login, model.Password, false);

                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false);
        }

        public ActionResult Logout()
        {
            _membershipService.LogoutCurrentUser(null);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult RegistrationFinish(RegisterModelPoco model)
        {
            if (ModelState.IsValid && !_usersService.EmailIsExist(model.Email))
            {
                var user = new User
                    {
                        Email = model.Email,
                        Password = model.Password.Md5(),
                        IsConfirmed = false,
                        RegisterDate = DateTime.Now,
                        LastVisitDate = DateTime.Now
                    };

                _usersService.Insert(user);

                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmailIsExist(string email)
        {
            var isExisting = _usersService.EmailIsExist(email);

            return Json(new {result = isExisting}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PhoneIsBusy(string phone)
        {
            return Json(null);
        }

        public JsonResult IsAuthenticated()
        {
            return Json(HttpContext.User.Identity.IsAuthenticated, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _usersService.Dispose();
            _membershipService.Dispose();
            base.Dispose(disposing);
        }
    }
}
