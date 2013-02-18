using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportSystem.Area.Web.Models.Account;
using TransportSystem.Domain;
using TransportSystem.Logics.Infrastructure.Extensions;
using TransportSystem.Logics.Interfaces.Membership;
using TransportSystem.Logics.Interfaces.SMS;

namespace TransportSystem.Area.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsersService _usersService;

        private readonly IMembershipService _membershipService;

        private readonly ISMS _smsService;

        public AccountController(
            IUsersService usersService,
            IMembershipService membershipService,
            ISMS smsService)
        {
            _usersService = usersService;
            _membershipService = membershipService;
            _smsService = smsService;
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
            if (!string.IsNullOrEmpty(model.Phone) && Session["VerificationCode"] != null && Session["VerificationCode"].ToString() != model.Code)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid && !_usersService.EmailIsExist(model.Email))
            {
                var phone = model.Phone;

                if (phone != null)
                {
                    // страшный костыль
                    // алиасы для России
                    phone = phone.Replace("+78", "78");
                    phone = phone.Replace("+79", "79");
                    phone = phone.First() == '8' ? "7" + phone.Substring(1) : phone;

                    // алиас для Казахстана
                    phone = phone.Replace("+77", "77");
                }

                var user = new User
                    {
                        Email = model.Email,
                        Password = model.Password.Md5(),
                        Phone = phone,
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

        public JsonResult PhoneIsBusy(string phonenumber)
        {
            var isExisting = _usersService.PhoneIsExist(phonenumber);

            return Json(new {result = isExisting}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendVerificationCode(string phonenumber)
        {
            var rng = new Random();
            var first = rng.Next(10);
            var second = rng.Next(10);
            var third = rng.Next(10);
            var fourth = rng.Next(10);
            var code = string.Format("{0}{1}{2}{3}", first, second, third, fourth);

            Session["VerificationCode"] = code;

            // страшный костыль
            // алиасы для России
            phonenumber = phonenumber.Replace("+78", "78");
            phonenumber = phonenumber.Replace("+79", "79");
            phonenumber = phonenumber.First() == '8' ? "7" + phonenumber.Substring(1) : phonenumber;

            // алиас для Казахстана
            phonenumber = phonenumber.Replace("+77", "77");

            _smsService.SendMessage(phonenumber, string.Format("Код подтверждения: {0}", code));

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckVerificationCode(string code)
        {
            return Json(Session["VerificationCode"].ToString() == code ? new { result = true } : new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsAuthenticated()
        {
            return Json(HttpContext.User.Identity.IsAuthenticated, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _usersService.Dispose();
            _membershipService.Dispose();
            _smsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
