using BPal.Business.Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BPal.Business.Portal.Core.Models
{
    public class Account : ModifiableEntity
    {
        [Key]
        public string AccountId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string CountryCode { get; set; }

        public string ImageUrl { get; set; }

        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [StringLength(15), Required]
        public string PhoneNumber { get; set; }

        [ForeignKey("AddressId")]
        public string AddressId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Required]
        public AccountType Type { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

        public string ActivationToken { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public DateTime? LastLogIn { get; set; }

        public string LastLoginLocation { get; set; }

    }
}
