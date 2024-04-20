using Interfaces;
using System.Net.Mail;
using System.Net;
using Extensions;

namespace Communication
{
    public class Exchange365SendEmail : IServerSendMessage
    {
        public readonly Exchange365ServerDetails Exchange365ServerDetails;

        public readonly Exchange365ServerAuthentication Exchange365ServerAuthentication;

        public string From { get; set; }

        public Exchange365SendEmail(IServerDetails serverDetails, IServerAuthentication serverAuthentication)
        {
            Exchange365ServerDetails = (Exchange365ServerDetails)serverDetails;
            Exchange365ServerAuthentication = (Exchange365ServerAuthentication)serverAuthentication;
        }

        public async Task Send(string subject, string messageContent, Dictionary<string, string> mergeFields, IEnumerable<IRecipient> recipients)
        {
            mergeFields.Add("{{CURRENT_YEAR}}", DateTime.Now.Year.ToString());
            string body = messageContent.PerformMerge(mergeFields);
            using SmtpClient smtpClient = new SmtpClient(Exchange365ServerDetails.Server, Exchange365ServerDetails.ServerPort);
            smtpClient.EnableSsl = Exchange365ServerDetails.UseSSL;
            smtpClient.Credentials = new NetworkCredential(Exchange365ServerAuthentication.Username, Exchange365ServerAuthentication.Password);
            using MailMessage smtpMessage = new MailMessage();
            smtpMessage.From = new MailAddress(From);
            smtpMessage.IsBodyHtml = true;
            smtpMessage.Body = body;
            smtpMessage.Subject = subject;
            smtpMessage.To.Clear();
            smtpMessage.Bcc.Clear();
            foreach (IRecipient recipient in recipients)
            {
                smtpMessage.To.Add(new MailAddress(recipient.Contact, recipient.Name));
            }

            await smtpClient.SendMailAsync(smtpMessage);
        }
    }
}
