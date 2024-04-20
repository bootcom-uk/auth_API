
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class User : BaseModel
    {

        public ObjectId Id { get; set; }

        public Guid UserId { get; set; }

        public string EmailAddress { get; set; }

        public string ProfileImage { get; set; }

        public bool EmailAddressConfirmed { get; set; }

        public DateTime? EmailAddressConfirmedDate { get; set; }

        public string Name { get; set; }

        public DateTime? DateCreated { get; set; }

    }
}
