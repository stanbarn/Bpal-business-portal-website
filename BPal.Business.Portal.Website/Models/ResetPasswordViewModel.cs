using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Models
{
    public class ResetPasswordViewModel
    {
        [Required, Display(Name = "Password")]
        public string Password { get; set; }

        [Required, Compare("Password", ErrorMessage = "Passwords Do Not Match"), Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Account { get; set; }
    }
}
