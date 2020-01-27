using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Models
{
    public class SignInModel
    {
        [EmailAddress]
        [Required, Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
