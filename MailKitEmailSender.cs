using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralCargoSystem.Models;
using GeneralCargoSystem.Data;

namespace GeneralCargoSystem.Utility
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly ApplicationDbContext _context;
        public MailKitEmailSender(IOptions<MailKitEmailSenderOptions> options, ApplicationDbContext context)
        {
            this.Options = options.Value;
            _context = context;
        }

        public MailKitEmailSenderOptions Options { get; set; }

        public Task SendEmailAsync(string email, string subject, string message)
        {

            return Execute(email, subject, message);
        }

        public Task Execute(string to, string subject, string message)
        {
            // create message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(Options.Sender_EMail);
            if (!string.IsNullOrEmpty(Options.Sender_Name))
                email.Sender.Name = Options.Sender_Name;
            email.From.Add(email.Sender);
            string[] Multi = to.Split(',');
            foreach (string MultiEmailId in Multi)
            {
                email.To.Add(MailboxAddress.Parse(MultiEmailId));

            }

            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = message };

            // send email
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(Options.Host_Address, Options.Host_Port, Options.Host_SecureSocketOptions);
                //smtp.Authenticate(Options.Host_Username, Options.Host_Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            var findUsername = _context.ApplicationUsers.Where(a => a.Email == to).FirstOrDefault()?.FirstName;

            var log = new Logs
            {
                UserEmail = to,
                UserName = findUsername,
                LogType = Enums.SentEmail,
                AffectedTable = "Users",
                DateTime = DateTime.Now
            };
            _context.Logs.Add(log);
            _context.SaveChanges();

            return Task.FromResult(true);
        }

    }
}
