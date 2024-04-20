using Interfaces;

namespace Communication
{
    public class Exchange365ServerDetails : IServerDetails
    {
        public string Server { get; set; }

        public int ServerPort { get; set; }

        public bool UseSSL { get; set; }
    }
}
