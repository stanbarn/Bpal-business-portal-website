using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Models
{
    public class AccountModel
    {
        public string AccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string EmailAddress { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public CountryModel Country { get; set; }

        public GalleryModel Gallery { get; set; }

        public DateTime? LastLogIn { get; set; }

        public string LastLoginLocation { get; set; }
    }
}
