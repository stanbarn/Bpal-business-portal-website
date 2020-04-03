using BPal.Business.Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class Account
    {
        public string AccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CountryCode { get; set; }

        public string ImageUrl { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string AddressId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public AccountType Type { get; set; }

        public AccountStatus Status { get; set; }

        public string ActivationToken { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime? LastLogIn { get; set; }

        public string LastLoginLocation { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
