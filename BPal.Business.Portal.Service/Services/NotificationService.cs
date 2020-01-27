using BPal.Business.Portal.Core.Services;
using Microsoft.Extensions.Configuration;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Service.Services
{
    public class NotificationService : INotificationService
    {
        IConfiguration _configuration;
        IRestRequest _restRequest;
        IRestClient _restClient;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string fromEmail, string fromName, List<EmailAddress> toEmails, string subject, string message)
        {
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, toEmails, subject, "", message, false);

            var response = await client.SendEmailAsync(msg);
        }

    }
}
