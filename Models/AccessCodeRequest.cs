using MongoDB.Bson;

namespace Models
{
    public class AccessCodeRequest : BaseModel
    {

        public ObjectId Id { get; set; }

        public string EmailAddress { get; set; }

        public string QuickAccessCode { get; set; }

        public Guid LoginCode { get; set; }

        public DateTime AccessCodeExpires { get; set; }

        public Guid DeviceId { get; set; }

    }
}
