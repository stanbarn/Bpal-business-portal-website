using BPal.Business.Portal.Website.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Helpers
{
    public static class SessionManager
    {
        public static HttpContext HttpContext => HttpHelper.HttpContext;

        public static List<Claim> UserClaims => HttpContext.User.Claims.ToList();

        #region account details

        public static string AccountId => (UserClaims.FirstOrDefault(x => x.Type.Equals("AccountId", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("AccountId", StringComparison.OrdinalIgnoreCase)).Value).Trim();

        public static string EmailAddress => (UserClaims.FirstOrDefault(x => x.Type.Equals("EmailAddress", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("EmailAddress", StringComparison.OrdinalIgnoreCase)).Value).Trim();

        public static string CountryCode => (UserClaims.FirstOrDefault(x => x.Type.Equals("CountryCode", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("CountryCode", StringComparison.OrdinalIgnoreCase)).Value).Trim();


        public static string CurrencyCode => (UserClaims.FirstOrDefault(x => x.Type.Equals("CurrencyCode", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("CurrencyCode", StringComparison.OrdinalIgnoreCase)).Value).Trim();

        public static string CurrencyName => UserClaims.FirstOrDefault(x => x.Type.Equals("CurrencyName", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("CurrencyName", StringComparison.OrdinalIgnoreCase)).Value;

        public static string FirstName => UserClaims.FirstOrDefault(x => x.Type.Equals("FirstName", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("FirstName", StringComparison.OrdinalIgnoreCase)).Value;

        public static string LastName => UserClaims.FirstOrDefault(x => x.Type.Equals("LastName", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("LastName", StringComparison.OrdinalIgnoreCase)).Value;

        public static string Name => string.Format("{0} {1}", LastName, FirstName);


        public static string ImageUri => UserClaims.FirstOrDefault(x => x.Type.Equals("ImageUri", StringComparison.OrdinalIgnoreCase)) == null ? string.Empty : UserClaims.FirstOrDefault(x => x.Type.Equals("ImageUri", StringComparison.OrdinalIgnoreCase)).Value;

        #endregion

        #region registration info
        public static bool IsAuthenticated
        {
            get
            {
                return bool.Parse(UserClaims.FirstOrDefault(x => x.Type.Equals("IsAuthenticated", StringComparison.OrdinalIgnoreCase)) == null ? bool.FalseString : string.IsNullOrWhiteSpace(UserClaims.FirstOrDefault(x => x.Type.Equals("IsAuthenticated", StringComparison.OrdinalIgnoreCase)).Value) ? bool.FalseString : UserClaims.FirstOrDefault(x => x.Type.Equals("IsAuthenticated", StringComparison.OrdinalIgnoreCase)).Value);
            }
        }

        public static bool IsAccountCreated => bool.Parse(UserClaims.FirstOrDefault(x => x.Type.Equals("IsAccountCreated", StringComparison.OrdinalIgnoreCase)) == null ? bool.FalseString : string.IsNullOrWhiteSpace(UserClaims.FirstOrDefault(x => x.Type.Equals("IsAccountCreated", StringComparison.OrdinalIgnoreCase)).Value) ? bool.FalseString : UserClaims.FirstOrDefault(x => x.Type.Equals("IsAccountCreated", StringComparison.OrdinalIgnoreCase)).Value);

        #endregion

        public static async Task UpdateClaim(AuthClaim authClaim)
        {
            if (HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(authClaim.ClaimName, StringComparison.OrdinalIgnoreCase)) != null)
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(HttpContext.User.Identity);

                claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(authClaim.ClaimName));
                claimsIdentity.AddClaim(new Claim(authClaim.ClaimName, authClaim.ClaimValue));

                await SignOutAsync();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.IsPersistent) == null ? false : bool.Parse(claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.IsPersistent).Value) });
            }
        }

        public static async Task AddClaim(AuthClaim authClaim)
        {
            if (HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(authClaim.ClaimName, StringComparison.OrdinalIgnoreCase)) == null)
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(HttpContext.User.Identity);

                claimsIdentity.AddClaim(new Claim(authClaim.ClaimName, authClaim.ClaimValue));

                await SignOutAsync();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.IsPersistent) == null ? false : bool.Parse(claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.IsPersistent).Value) });
            }
        }

        public static async Task RemoveClaim(AuthClaim authClaim)
        {
            if (HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(authClaim.ClaimName, StringComparison.OrdinalIgnoreCase)) != null)
            {
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(HttpContext.User.Identity);

                claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(authClaim.ClaimName));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.IsPersistent) == null ? false : bool.Parse(claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.IsPersistent).Value) });
            }
        }

        public static async Task UpdateClaims(List<AuthClaim> authClaims)
        {
            foreach (var claim in authClaims)
            {
                await UpdateClaim(claim);
            }
        }

        public static async Task AddClaims(List<AuthClaim> authClaims)
        {
            foreach (var claim in authClaims)
            {
                await AddClaim(claim);
            }
        }

        public static async Task RemoveClaims(List<AuthClaim> authClaims)
        {
            foreach (var claim in authClaims)
            {
                await RemoveClaim(claim);
            }
        }

        public static async Task SignInAsync(string username, List<AuthClaim> authClaims, bool isPersistent = false)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var identityClaims = new List<Claim>
            {
                new Claim(ClaimTypes.IsPersistent, isPersistent.ToString()),
                new Claim(ClaimTypes.NameIdentifier, username),
            };

            authClaims.ForEach(c => identityClaims.Add(new Claim(c.ClaimName, c.ClaimValue)));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(identityClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = isPersistent, ExpiresUtc = DateTimeOffset.Now.AddDays(1), });
        }

        public static async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
