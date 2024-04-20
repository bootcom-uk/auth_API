using Models;
using MongoDB.Bson;
using System.Reflection;

namespace Interfaces
{
    public interface IDatabaseService
    {

        Task<Models.User?> GetUserById(ObjectId? id);

        Task<Models.User?> GetUserByEmailAddress(string emailAddress);

        Task CreateUser(string emailAddress);

        Task<AccessCodeRequest?> SendAuthToken(string emailAddress, Guid deviceId);

        Task<User?> ValidateQuickAccessCode(string accessCode, Guid deviceId);

        Task<RefreshToken?> CreateRefreshToken(RefreshToken refreshToken);

        Task<bool> ValidateRefreshToken(string token, string refreshToken, Guid deviceId);

        Task<User?> DeleteRefreshToken(string refreshToken);
    }
}
