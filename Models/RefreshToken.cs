using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class RefreshToken
    {
        public ObjectId Id { get; set; }

        public ObjectId UserId { get; set; }

        public string Token { get; set; }

        public DateTime Expiry { get; set; }

        public string OriginalToken { get; set; }

        public Guid DeviceId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
