using BPal.Business.Portal.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Helpers
{
    public static class AccountHelper
    {
        public static void ValidatePassword(string input, int minlength = 10)
        {
            if (minlength < 4)
            {
                throw new AccountValidationException("Password Minimum Length Cannot Less Than 4");
            }

            // not empty
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new AccountValidationException("Password Is Empty");
            }

            // not white space
            if (input.Any(x => char.IsWhiteSpace(x)))
            {
                throw new AccountValidationException("Password Cannot Contain Whitespace");
            }

            // not too short
            if (input.Length < minlength)
            {
                throw new AccountValidationException($"Password Length Cannot Cannot Be Less Than {minlength}");
            }

            // has numbers & chars
            if (!input.Any(x => char.IsLetterOrDigit(x)))
            {
                throw new AccountValidationException("Password Must Contain Alpha-Numeric Character");
            }

            // has upper  case
            if (!input.Any(x => char.IsUpper(x)))
            {
                throw new AccountValidationException("Password Must Contain Alphabetic Lower Case Characters");
            }

            // has lower case
            if (!input.Any(x => char.IsLower(x)))
            {
                throw new AccountValidationException("Password Must Contain Alphabetic Upper Case Characters");
            }

        }

        public static string GeneratePasswordHash(string password)
        {
            using (var hashAlgorith = SHA512.Create())
            {
                return Convert.ToBase64String(hashAlgorith.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        public static string GenerateEmailConfirmationToken(string accountId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{accountId}-{Guid.NewGuid().ToString()}"));
        }
    }
}
