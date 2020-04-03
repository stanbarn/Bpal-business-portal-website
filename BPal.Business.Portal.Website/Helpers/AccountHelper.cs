using BPal.Business.Portal.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static string GenerateEmailConfirmationToken(string accountId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{accountId}-{Guid.NewGuid().ToString()}"));
        }

        public static string GeneratePasswordHash(string password)
        {
            try
            {
                if (password != "")
                {
                    byte[] clearBytes = Encoding.Unicode.GetBytes(password);
                    using (Aes encryptor = Aes.Create())
                    {
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes("bpal@cloud.com2019", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.Close();
                            }
                            password = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch
            {
                password = "";
            }
            return password;
        }
    }
}
