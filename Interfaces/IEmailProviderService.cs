namespace Interfaces
{
    public interface IEmailProviderService
    {

        Task Send(string from, string subject, string messageContent, Dictionary<string, string> mergeFields, IEnumerable<IRecipient> recipients);

    }

}
