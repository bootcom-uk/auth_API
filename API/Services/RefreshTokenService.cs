using Extensions;
using Interfaces;
using Models;
using MongoDB.Bson;

namespace API.Services
{
    public class RefreshTokenService
    {

        private readonly IConfiguration _configuration;

        public RefreshTokenService(IConfiguration configuration) { 
            _configuration = configuration;
        }

        public RefreshToken CreateRefreshToken(User user, string token, Guid deviceId)
        {
            
            var refreshToken = new RefreshToken()
            {
                UserId = user.Id,
                OriginalToken = token,
                Expiry = DateTime.UtcNow.AddDays(60),
                Token = string.Empty.GenerateRefreshToken(),
                DeviceId = deviceId,
                DateCreated = DateTime.UtcNow,
                Id = ObjectId.GenerateNewId()
            };

            return refreshToken;
        }

    }

}