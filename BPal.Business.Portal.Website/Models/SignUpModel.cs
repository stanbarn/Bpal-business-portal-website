using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Models
{
    public class SignUpModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(15), Display(Name = "Phone Number"), Required]
        public string PhoneNumber { get; set; }

        [EmailAddress, Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password"), Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Agent Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Agent Address Line 2")]
        public string AddressLine2 { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Compnay Address Line 1")]
        public string CompanyAddressLine1 { get; set; }

        [Display(Name = "Compnay Address Line 2")]
        public string CompanyAddressLine2 { get; set; }
    }
}
