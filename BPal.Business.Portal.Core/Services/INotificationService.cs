using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Core.Services
{
    public interface INotificationService
    {
        Task SendEmail(string fromEmail, string fromName, List<EmailAddress> toEmails, string subject, string message);
    }
}
