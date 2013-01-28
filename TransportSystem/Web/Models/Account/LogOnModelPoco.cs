using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportSystem.Area.Web.Models.Account
{
    public class LogOnModelPoco
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        public bool? RememberMe { get; set; }
    }
}