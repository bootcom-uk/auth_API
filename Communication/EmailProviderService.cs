using Interfaces;
using Microsoft.Extensions.Configuration;

namespace Communication
{
    public class EmailProviderService : IEmailProviderService
    {

        private readonly IConfiguration Configuration;

        public EmailProviderService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task Send(string from, string subject, string messageContent, Dictionary<string, string> mergeFields, IEnumerable<IRecipient> recipients)
        {
            var exchange365SendEmail = new Exchange365SendEmail((Exchange365ServerDetails)new()
            {
                Server = Configuration["EXCHANGE_EMAIL_SERVER"],
                ServerPort = int.Parse(Configuration["EXCHANGE_EMAIL_SERVER_PORT"]),
                UseSSL = bool.Parse(Configuration["EXCHANGE_EMAIL_SERVER_USE_SSL"])
            }, (Exchange365ServerAuthentication)new()
            {
                Username = Configuration["EXCHANGE_EMAIL_NOREPLY_USERNAME"],
                Password = Configuration["EXCHANGE_EMAIL_NOREPLY_PASSWORD"]
            });

            exchange365SendEmail.From = from;

            await exchange365SendEmail.Send(subject, messageContent, mergeFields, recipients);
        }
    }
}
