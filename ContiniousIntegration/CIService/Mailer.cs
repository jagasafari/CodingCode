namespace CIService
{
    using System;
    using System.Net;
    using System.Net.Mail;

    public class Mailer
    {
        private readonly CiMailConfiguration _mailConfiguration;

        public Mailer(CiMailConfiguration mailConfiguration)
        {
            _mailConfiguration = mailConfiguration;
        }

        public void SendReportEmail(string testsResult)
        {
            using (var smtpClient = CreateSmtClient())
            {
                smtpClient.Send(CreateMessageEmail(testsResult));
            }
        }

        private MailMessage CreateMessageEmail(string testsResult)
        {
            return new MailMessage(_mailConfiguration.Sender,
                _mailConfiguration.Receiver, $"CI Report {DateTime.UtcNow}",
                testsResult);
        }

        private SmtpClient CreateSmtClient()
        {
            return new SmtpClient(_mailConfiguration.SmtpHost,
                _mailConfiguration.SmtpPort)
            {
                EnableSsl = true,
                Credentials =
                    new NetworkCredential(_mailConfiguration.Sender,
                        _mailConfiguration.Password)
            };
        }
    }
}