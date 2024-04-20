namespace Interfaces
{
    public interface IServerSendMessage
    {
        string From { get; set; }

        Task Send(string subject, string messageContent, Dictionary<string, string> mergeFields, IEnumerable<IRecipient> recipients);
    }
}
