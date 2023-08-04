using be.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Crypto.Generators;

namespace be.Services
{
    public class EmailService
    {
        private static EmailService instance;
        public static EmailService Instance
        {
            get { if (instance == null) instance = new EmailService(); return EmailService.instance; }
            private set { EmailService.instance = value; }
        }
        public async Task<bool> SendMail(string mail, int type, string value = null)
        {
            try
            {
                // With type == 1, you are reset password
                // With type == 2, you are send mail verify
                string _text = "";
                if (type == 1)
                {
                    _text = EmailHelper.Instance.BodyReset(value);
                }
                else
                {
                    _text = EmailHelper.Instance.Body(value);
                }
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("vmhsky7@gmail.com"));
                email.To.Add(MailboxAddress.Parse(mail));
                email.Subject = "Confirm account";
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = _text
                };
                var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("vmhsky7@gmail.com", "dvjgqyaqzugkciif");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }


    }
}
