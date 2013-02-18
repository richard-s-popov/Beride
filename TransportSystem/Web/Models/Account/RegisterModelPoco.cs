using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransportSystem.Area.Web.Models.Account
{
    public class RegisterModelPoco
    {
        [RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Code { get; set; }

        [Required]
        [RegularExpression("^[^\f\n\r\t]{6,32}$")]
        public string Password { get; set; }
    }
}