using Interfaces;

namespace Communication
{
    public class Exchange365EmailRecipient : IRecipient
    {
        public string Name { get; set; }

        public string Contact { get; set; }
    }
}
