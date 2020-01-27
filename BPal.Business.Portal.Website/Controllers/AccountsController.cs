using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BPal.Business.Portal.Core.Enums;
using BPal.Business.Portal.Core.Exceptions;
using BPal.Business.Portal.Core.Models;
using BPal.Business.Portal.Core.Services;
using BPal.Business.Portal.Website.Helpers;
using BPal.Business.Portal.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;

namespace BPal.Business.Portal.Website.Controllers
{
    public class AccountsController : Controller
    {
        IAccountService _accountService;
        INotificationService _notificationService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AccountsController(IAccountService accountService, INotificationService notificationService,  IHostingEnvironment hostingEnvironment)
        {
            _accountService = accountService;
            _notificationService = notificationService;
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> Index()
        {
            var agentAndAccount = await _accountService.GetAsync(SessionManager.AccountId);
            if (agentAndAccount?.AccountId == null)
            {
                return RedirectToAction("SignIn");
            }

            return View(agentAndAccount);
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var account = await _accountService.GetAsync(SessionManager.AccountId);

            return View(account);
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var account = await _accountService.GetAsync(SessionManager.AccountId);

            return View(account.ToViewModel());
        }

        [Authorize, HttpGet]
        public async Task<IActionResult> Settings()
        {
            var account = await _accountService.GetAsync(SessionManager.AccountId);
            return View(account);
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            if (SessionManager.IsAuthenticated)
            {
                if (returnUrl != null)
                    return RedirectToAction(returnUrl);

                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel signInModel, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var pass = AccountHelper.GeneratePasswordHash(signInModel.Password);

                var account = await _accountService.AuthenticateAsync(signInModel.EmailAddress, AccountHelper.GeneratePasswordHash(signInModel.Password));

                if (account != null)
                {
                    if (!account.IsEmailConfirmed)
                    {
                        ModelState.AddModelError("", "Sorry! Account Pending Confirmation");
                        return View(signInModel);
                    }
                    var authClaims = new List<AuthClaim> {
                        new AuthClaim{ ClaimName = "AccountId", ClaimValue = account.AccountId },
                        new AuthClaim{ ClaimName = "EmailAddress",ClaimValue = account.EmailAddress },
                        new AuthClaim { ClaimName = ClaimTypes.Role, ClaimValue = account.Type.ToString() },
                        new AuthClaim { ClaimName = "FirstName", ClaimValue = account.FirstName },
                        new AuthClaim { ClaimName = "LastName", ClaimValue = account.LastName },
                        new AuthClaim { ClaimName = "IsAuthenticated", ClaimValue = bool.TrueString },
                        new AuthClaim { ClaimName = ClaimTypes.Name, ClaimValue = $"{account.FirstName} {account.LastName}" }
                    };

                    //signIn
                    await SessionManager.SignInAsync(account.EmailAddress, authClaims, signInModel.RememberMe);
                    account.LastLogIn = DateTime.UtcNow;

                    await _accountService.UpdateAsync(account);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Sorry! Authentication Failed");
                }

            }

            return View(signInModel);


        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            if (SessionManager.IsAuthenticated)
                return RedirectToAction("Index");

            return View(new SignUpModel());

        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ActivateAccount()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorList = (from item in ModelState
                                     where item.Value.Errors.Any()
                                     select item.Value.Errors[0].ErrorMessage).ToList();
                    return Json(new
                    {
                        response = "notsuccess",
                        message = errorList
                    });
                }


                if (model.NewPassword != model.ConfirmPassword)
                    return Json(new
                    {
                        response = "notsuccess",
                        message = "Passwords do not much. Make sure new password and confirm password fields match."
                    });

                var account = await _accountService.GetAsync(SessionManager.AccountId);
                if (account == null)
                    return Json(new
                    {
                        response = "notsuccess",
                        message = "account you are trying to update was not found."
                    });

                if (account.Password != AccountHelper.GeneratePasswordHash(model.CurrentPassword))
                    return Json(new
                    {
                        response = "notsuccess",
                        message = "Provided current password is incorrect."
                    });

                AccountHelper.ValidatePassword(model.NewPassword, 8);

                account.Password = AccountHelper.GeneratePasswordHash(model.NewPassword);
                account.ModifiedBy = SessionManager.AccountId;
                account.ModifiedOn = DateTime.UtcNow;

                var updatedAccount = await _accountService.UpdateAsync(account);
                return Json(new
                {
                    response = "success",
                    message = "password updated successfully."
                });
            }
            catch (AccountValidationException exception)
            {
                return Json(new
                {
                    response = "notsuccess",
                    message = exception.Message
                });
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    response = "notsuccess",
                    message = "Errors occured. Contact support team."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("ResetPasswordError");
            }

            var account = await _accountService.GetAsync(userId);
            if (account == null)
            {
                return RedirectToAction("ConfirmEmail");
            }
            if (account.ActivationToken != code)
            {
                return View("ResetPasswordConfirmation");
            }

            if (!account.IsEmailConfirmed)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            ViewBag.AccountId = account.AccountId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            ViewBag.AccountId = resetPassword?.Account;
            if (ModelState.IsValid)
            {
                if (resetPassword.Password != resetPassword.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View(resetPassword);
                }
                var account = await _accountService.GetAsync(resetPassword.Account);
                if (account == null)
                {
                    return View(resetPassword);
                }

                if (!account.IsEmailConfirmed || account.Status != AccountStatus.ACTIVE)
                {
                    ModelState.AddModelError("", "Account is not Active");
                    return View(resetPassword);
                }

                AccountHelper.ValidatePassword(resetPassword.Password);
                account.Password = AccountHelper.GeneratePasswordHash(resetPassword.Password);
                account.ActivationToken = null;
                account.ModifiedBy = $"{account.FirstName} {account.LastName}";
                account.ModifiedOn = DateTime.UtcNow;

                var updatedAccount = await _accountService.UpdateAsync(account);
                if (account == null)
                {
                    ModelState.AddModelError("", "Errors Occurred");
                    return View(resetPassword);
                }

                //send password reset email notification
                await _notificationService.SendEmail("noreply@murcomhomes.com", "Murcom Homes", new List<EmailAddress> {
                            new EmailAddress(account.EmailAddress)
                        }, "Murcom Homes", GenerateAccountChangeEmailTemplate("Account Password Reset!", $"Hi { account.FirstName} { account.LastName}, Your Account password was successfully reset"));

                return RedirectToAction("SignIn");
            }

            return View(resetPassword);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await _accountService.GetByEmailAsync(model.EmailAddress);
                if (account == null || !account.IsEmailConfirmed)
                {
                    ViewBag.ResetErrorMessage = "Sorry! Password can not be reset because account does not exist or its not active.";
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmationError");
                }

                var code = AccountHelper.GenerateEmailConfirmationToken(account.AccountId);
                account.ActivationToken = code;

                await _accountService.UpdateAsync(account);

                var callbackUrl = Url.Action("ResetPassword", "Accounts",
            new { UserId = account.AccountId, code = code }, protocol: Request.Scheme);

                //send email notification
                await _notificationService.SendEmail("noreply@whisperssafarisuganda.com", "Elephant Whispers Safaris Uganda", new List<EmailAddress> {
                            new EmailAddress(account.EmailAddress)
                        }, "Elephant Whispers Safaris Uganda", GenerateResetPasswordEmailTemplate(account, callbackUrl));

                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProfile(AccountModel model)
        {
            var account = new Account();
            if (ModelState.IsValid)
            {
                account = await _accountService.GetAsync(model.AccountId);
                if (account == null || !account.IsEmailConfirmed)
                {
                    ViewBag.ResetErrorMessage = "Sorry! Password can not be reset because account does not exist or its not active.";
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmationError");
                }

                account.FirstName = model.FirstName;
                account.LastName = model.LastName;
                account.ModifiedBy = account.AccountId;
                account.ModifiedOn = DateTime.UtcNow;

                await _accountService.UpdateAsync(account);

                //send email notification
                await _notificationService.SendEmail("noreply@murcomhomes.com", "Murcom Homes", new List<EmailAddress> {
                            new EmailAddress(account.EmailAddress)
                        }, "Murcom Homes", GenerateAccountChangeEmailTemplate("Account Update Notification", $"Hello {account.FirstName}, Your account was updated at {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm tt")} GMT. If you did not do this, Kindly visit your Account and change the password"));

                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var account = await _accountService.GetAsync(userId);
            if (account == null)
            {
                return RedirectToAction("ConfirmEmail");
            }

            if (account.IsEmailConfirmed)
            {
                return RedirectToAction("ConfirmEmail");
            }

            if (account.CreatedOn.AddDays(1) < DateTime.UtcNow)
            {
                return RedirectToAction("ConfirmEmail");
            }

            account.Status = AccountStatus.ACTIVE;
            var activatedAccount = await _accountService.ActivateAccountAsync(account.EmailAddress, code);
            if (account == null)
            {
                return View("ConfirmEmail");
            }

            return View("ConfirmEmail");
        }

        [Authorize]
        public async Task<ActionResult> SignOut()
        {
            await SessionManager.SignOutAsync();

            return RedirectToAction("SignIn");
        }

        #region Helper Methods

        private static string GenerateActivationEmailTemplate(Account account, string url)
        {
            var message = @"<!doctype html>
<html xmlns='https://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
    <title></title>
    <!--[if !mso]>-->
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <!--<![endif]-->
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <style type='text/css'>
        #outlook a {
            padding: 0;
        }
        
        .ReadMsgBody {
            width: 100%;
        }
        
        .ExternalClass {
            width: 100%;
        }
        
        .ExternalClass * {
            line-height: 100%;
        }
        
        body {
            margin: 0;
            padding: 0;
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
        }
        
        table,
        td {
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
        }
        
        img {
            border: 0;
            height: auto;
            line-height: 100%;
            outline: none;
            text-decoration: none;
            -ms-interpolation-mode: bicubic;
        }
        
        p {
            display: block;
            margin: 13px 0;
        }
    </style>
    <!--[if !mso]><!-->
    <style type='text/css'>
        @media only screen and (max-width:480px) {
            @-ms-viewport {
                width: 320px;
            }
            @viewport {
                width: 320px;
            }
        }
    </style>
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700' rel='stylesheet' type='text/css'>
    <style type='text/css'>
        @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);
    </style>
    <!--<![endif]-->
    <style type='text/css'>
        @media only screen and (min-width:480px) {
            .mj-column-per-100 {
                width: 100%!important;
            }
        }
    </style>
    <style type='text/css'>
        @media only screen and (max-width:480px) {
            .mj-hero-content {
                width: 100% !important;
            }
        }
    </style>
</head>

<body style='background: #f5f6fa;'>
    <div style='background-color:#f5f6fa;'>
        <div style='margin:0px auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;padding-bottom:20px;padding-top:30px;'>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0 auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='width:100%;'>
                <tbody>
                    <tr style='vertical-align:top;'>
                        <td height='100' style='background-color:#252525'>
                            <img src='https://dalelo.azurewebsites.net/images/logo.png' style='margin:auto;display:flex;padding:15px 0;'/>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0px auto;max-width:600px;background:#fff;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;background:#fff;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;'>
                            
                            <div class='mj-column-per-100 outlook-group-fix' style='vertical-align:top;display:inline-block;direction:ltr;font-size:13px;text-align:left;width:100%;'>
                                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                    <tbody>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;' align='center'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:40px;text-align:center;'>
                                                    <h2 class='ks-header-h2' style='font-size: 30px; font-weight: 500; color: #333; margin-top: 0; margin-bottom: 0;'>
                                                        Welcome to Murcom Properties!
                                                    </h2>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;' align='left'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;text-align:left;'>
                                                    <p style='font-size: 14px; color: #333; margin: 5px 0;'>Hi " + account.FirstName + " " + account.LastName + @", Welcome to Murcom Properties. Great to have you on board.</p>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-top:20px;padding-bottom:20px;'>
                                                <p style='color: #333; font-size: 1px; margin: 0px auto; border-top: 1px solid #e6e6e6; width: 100%;'></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:0px;' align='left'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;text-align:left;'>
                                                    <h3 class='ks-header-h3' style='font-size: 24px; font-weight: 500; color: #333; margin-top: 0; margin-bottom: 10px;'>
                                                                            1. Verify your account
                                                                        </h3>
                                                    <p style='font-size: 14px; color: #333; margin: 5px 0;'>
                                                        To ensure you’re legitimate and not some fake person, please verify your account by clicking the button below.
                                                    </p>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:30px;' align='left'>
                                                <table role='presentation' cellpadding='0' cellspacing='0' style='border-collapse:separate;' align='left' border='0'>
                                                    <tbody>
                                                        <tr>
                                                            <td style='border:none;border-radius:2px;color:#fff;cursor:auto;padding:12px 30px;' align='center' valign='middle' bgcolor='#3a529b'>
                                                                <a href='" + url + @"' style='text-decoration:none;line-height:100%;background:#3a529b;color:#fff;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:14px;font-weight:500;text-transform:none;margin:0px;' target='_blank'>
                                                                Verify my account
                                                            </a></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:20px;'>
                                                <p style='color: #333; font-size: 1px; margin: 0px auto; border-top: 1px solid #e6e6e6; width: 100%;'>
                                                </p>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0px auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;'>
                            <div class='' style='cursor:auto;color:#858585;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:12px;line-height:22px;text-align:center;'>
                                <div class='ks-copyright' style='margin-bottom: 15px;'>
                                    Murcom Properties
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</body>

</html>";
            return message;
        }

        private static string GenerateResetPasswordEmailTemplate(Account account, string url)
        {
            var message = @"<!doctype html>
<html xmlns='https://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
    <title></title>
    <!--[if !mso]>-->
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <!--<![endif]-->
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <style type='text/css'>
        #outlook a {
            padding: 0;
        }
        
        .ReadMsgBody {
            width: 100%;
        }
        
        .ExternalClass {
            width: 100%;
        }
        
        .ExternalClass * {
            line-height: 100%;
        }
        
        body {
            margin: 0;
            padding: 0;
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
        }
        
        table,
        td {
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
        }
        
        img {
            border: 0;
            height: auto;
            line-height: 100%;
            outline: none;
            text-decoration: none;
            -ms-interpolation-mode: bicubic;
        }
        
        p {
            display: block;
            margin: 13px 0;
        }
    </style>
    <!--[if !mso]><!-->
    <style type='text/css'>
        @media only screen and (max-width:480px) {
            @-ms-viewport {
                width: 320px;
            }
            @viewport {
                width: 320px;
            }
        }
    </style>
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700' rel='stylesheet' type='text/css'>
    <style type='text/css'>
        @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);
    </style>
    <!--<![endif]-->
    <style type='text/css'>
        @media only screen and (min-width:480px) {
            .mj-column-per-100 {
                width: 100%!important;
            }
        }
    </style>
    <style type='text/css'>
        @media only screen and (max-width:480px) {
            .mj-hero-content {
                width: 100% !important;
            }
        }
    </style>
</head>

<body style='background: #f5f6fa;'>
    <div style='background-color:#f5f6fa;'>
        <div style='margin:0px auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;padding-bottom:20px;padding-top:30px;'>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0 auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='width:100%;'>
                <tbody>
                    <tr style='vertical-align:top;'>
                        <td height='100' style='background-color:#252525'>
                            <img src='https://dalelo.azurewebsites.net/images/logo.png' style='margin:auto;display:flex;padding:15px 0;'/>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0px auto;max-width:600px;background:#fff;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;background:#fff;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;'>
                            
                            <div class='mj-column-per-100 outlook-group-fix' style='vertical-align:top;display:inline-block;direction:ltr;font-size:13px;text-align:left;width:100%;'>
                                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                    <tbody>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;' align='center'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:40px;text-align:center;'>
                                                    <h2 class='ks-header-h2' style='font-size: 30px; font-weight: 500; color: #333; margin-top: 0; margin-bottom: 0;'>
                                                        Reset Password!
                                                    </h2>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;' align='left'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;text-align:left;'>
                                                    <p style='font-size: 14px; color: #333; margin: 5px 0;'>Hi " + account.FirstName + " " + account.LastName + @", You requested to reset your password.</p>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-top:20px;padding-bottom:20px;'>
                                                <p style='color: #333; font-size: 1px; margin: 0px auto; border-top: 1px solid #e6e6e6; width: 100%;'></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:0px;' align='left'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;text-align:left;'>
                                                    <h3 class='ks-header-h3' style='font-size: 24px; font-weight: 500; color: #333; margin-top: 0; margin-bottom: 10px;'>
                                                                            1. Reset your account password
                                                                        </h3>
                                                    <p style='font-size: 14px; color: #333; margin: 5px 0;'>
                                                        You forgot your password? Do not worry, click the link below to reset your password.
                                                    </p>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:30px;' align='left'>
                                                <table role='presentation' cellpadding='0' cellspacing='0' style='border-collapse:separate;' align='left' border='0'>
                                                    <tbody>
                                                        <tr>
                                                            <td style='border:none;border-radius:2px;color:#fff;cursor:auto;padding:12px 30px;' align='center' valign='middle' bgcolor='#3a529b'>
                                                                <a href='" + url + @"' style='text-decoration:none;line-height:100%;background:#3a529b;color:#fff;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:14px;font-weight:500;text-transform:none;margin:0px;' target='_blank'>
                                                                Reset password
                                                            </a></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:20px;'>
                                                <p style='color: #333; font-size: 1px; margin: 0px auto; border-top: 1px solid #e6e6e6; width: 100%;'>
                                                </p>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0px auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;'>
                            <div class='' style='cursor:auto;color:#858585;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:12px;line-height:22px;text-align:center;'>
                                <div class='ks-copyright' style='margin-bottom: 15px;'>
                                    Murcom Properties
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</body>

</html>";
            return message;
        }

        private static string GenerateAccountChangeEmailTemplate(string subject, string emailMessage)
        {
            var message = @"<!doctype html>
<html xmlns='https://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>

<head>
    <title></title>
    <!--[if !mso]>-->
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <!--<![endif]-->
    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>
    <style type='text/css'>
        #outlook a {
            padding: 0;
        }
        
        .ReadMsgBody {
            width: 100%;
        }
        
        .ExternalClass {
            width: 100%;
        }
        
        .ExternalClass * {
            line-height: 100%;
        }
        
        body {
            margin: 0;
            padding: 0;
            -webkit-text-size-adjust: 100%;
            -ms-text-size-adjust: 100%;
        }
        
        table,
        td {
            border-collapse: collapse;
            mso-table-lspace: 0pt;
            mso-table-rspace: 0pt;
        }
        
        img {
            border: 0;
            height: auto;
            line-height: 100%;
            outline: none;
            text-decoration: none;
            -ms-interpolation-mode: bicubic;
        }
        
        p {
            display: block;
            margin: 13px 0;
        }
    </style>
    <!--[if !mso]><!-->
    <style type='text/css'>
        @media only screen and (max-width:480px) {
            @-ms-viewport {
                width: 320px;
            }
            @viewport {
                width: 320px;
            }
        }
    </style>
    <link href='https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700' rel='stylesheet' type='text/css'>
    <style type='text/css'>
        @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);
    </style>
    <!--<![endif]-->
    <style type='text/css'>
        @media only screen and (min-width:480px) {
            .mj-column-per-100 {
                width: 100%!important;
            }
        }
    </style>
    <style type='text/css'>
        @media only screen and (max-width:480px) {
            .mj-hero-content {
                width: 100% !important;
            }
        }
    </style>
</head>

<body style='background: #f5f6fa;'>
    <div style='background-color:#f5f6fa;'>
        <div style='margin:0px auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;padding-bottom:20px;padding-top:30px;'>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0 auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='width:100%;'>
                <tbody>
                    <tr style='vertical-align:top;'>
                        <td height='100' style='background-color:#252525'>
                            <img src='https://dalelo.azurewebsites.net/images/logo.png' style='margin:auto;display:flex;padding:15px 0;'/>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0px auto;max-width:600px;background:#fff;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;background:#fff;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;'>
                            
                            <div class='mj-column-per-100 outlook-group-fix' style='vertical-align:top;display:inline-block;direction:ltr;font-size:13px;text-align:left;width:100%;'>
                                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' border='0'>
                                    <tbody>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;' align='center'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:40px;text-align:center;'>
                                                    <h2 class='ks-header-h2' style='font-size: 30px; font-weight: 500; color: #333; margin-top: 0; margin-bottom: 0;'>
                                                        " + subject + @"
                                                    </h2>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-top:20px;padding-bottom:20px;'>
                                                <p style='color: #333; font-size: 1px; margin: 0px auto; border-top: 1px solid #e6e6e6; width: 100%;'></p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:0px;' align='left'>
                                                <div class='' style='cursor:auto;color:#000000;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:22px;text-align:left;'>
                                                    <p style='font-size: 14px; color: #333; margin: 5px 0;'>
                                                        " + emailMessage + @"
                                                    </p>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style='word-break:break-word;font-size:0px;padding:10px 25px;padding-bottom:20px;'>
                                                <p style='color: #333; font-size: 1px; margin: 0px auto; border-top: 1px solid #e6e6e6; width: 100%;'>
                                                </p>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div style='margin:0px auto;max-width:600px;'>
            <table role='presentation' cellpadding='0' cellspacing='0' style='font-size:0px;width:100%;' align='center' border='0'>
                <tbody>
                    <tr>
                        <td style='text-align:center;vertical-align:top;direction:ltr;font-size:0px;padding:20px 0px;'>
                            <div class='' style='cursor:auto;color:#858585;font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:12px;line-height:22px;text-align:center;'>
                                <div class='ks-copyright' style='margin-bottom: 15px;'>
                                    Murcom Properties
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</body>

</html>";
            return message;
        }

        #endregion
    }
}