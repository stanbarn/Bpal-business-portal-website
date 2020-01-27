using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Models
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string EmailAddress { get; set; }
    }
}
