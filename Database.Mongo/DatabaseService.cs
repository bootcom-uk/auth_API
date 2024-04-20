using Interfaces;
using Microsoft.Extensions.Configuration;
using Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Net.Mail;

namespace Database.Mongo
{
    public class DatabaseService : IDatabaseService
    {

        private readonly IConfiguration Configuration;

        public DatabaseService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IMongoDatabase GetMongoDatabase()
        {
            var clientSettings = MongoClientSettings.FromConnectionString(Configuration["MONGO_IDENTITY_CONNECTION_STRING"]);

            clientSettings.LinqProvider = LinqProvider.V3;
            // clientSettings.GuidRepresentation = GuidRepresentation.Standard;

            var client = new MongoClient(clientSettings);

            return client.GetDatabase(Configuration["MONGO_IDENTITY_DATABASE_NAME"]);
        }

        public async Task CreateUser(string emailAddress)
        {

            var database = GetMongoDatabase();
            if (database is null) return;
            var usersCollection = database.GetCollection<Models.User>(nameof(Models.User));

           await usersCollection.InsertOneAsync(new()
            {
                EmailAddress = emailAddress,
                EmailAddressConfirmed = false,
                Id = ObjectId.GenerateNewId(),
                UserId = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow
            });
        }

        public async Task<User?> ValidateQuickAccessCode(string accessCode, Guid deviceId)
        {
            var database = GetMongoDatabase();
            if (database is null) return null;
            var accessCodeCollection = database.GetCollection<AccessCodeRequest>(nameof(AccessCodeRequest));

            // If we don't find the access code associated to the device return null, otherwise 
            // create our token and refresh token and send back
            var foundDocuments = accessCodeCollection.AsQueryable().Where(record => record.QuickAccessCode == accessCode && record.DeviceId == deviceId);
            if (foundDocuments.Count() != 1)
            {
                return null;
            }

            var requestDoc = await foundDocuments.FirstAsync();
            await accessCodeCollection.DeleteOneAsync(record => record.QuickAccessCode == accessCode && record.DeviceId == deviceId);

            var user = await GetUserByEmailAddress(requestDoc.EmailAddress);

            if(user is null)
            {
                return null;
            }


            return user;

        }

        public async Task<Models.User?> GetUserById(ObjectId? id)
        {
            if(id is null) return null;

            var database = GetMongoDatabase();
            if (database is null) return null;
            var usersCollection = database.GetCollection<Models.User>(nameof(Models.User));
            var users = await usersCollection.FindAsync(record => record.Id == id);
            return users.FirstOrDefault();
        }

        public async Task<Models.User?> GetUserByEmailAddress(string emailAddress)
        {
            var database = GetMongoDatabase();
            if (database is null) return null;
            var usersCollection = database.GetCollection<Models.User>(nameof(Models.User));
            var users = await usersCollection.FindAsync(record => record.EmailAddress == emailAddress);
            return users.FirstOrDefault();
        }

        public async Task<AccessCodeRequest?> SendAuthToken(string emailAddress, Guid deviceId)
        {
            var database = GetMongoDatabase();
            if (database is null) return null;
            var accessCodeCollection = database.GetCollection<AccessCodeRequest>(nameof(AccessCodeRequest));
            var accessCode = new AccessCodeRequest()
            {
                Id = ObjectId.GenerateNewId(),
                EmailAddress = emailAddress,
                QuickAccessCode = Extensions.Strings.RandomString(6, true, false, false),
                LoginCode = Guid.NewGuid(),
                AccessCodeExpires = DateTime.Now.AddMinutes(30),
                DeviceId = deviceId
            };
            await accessCodeCollection.InsertOneAsync(accessCode);
            return accessCode;
        }

        public async Task<RefreshToken?> CreateRefreshToken(RefreshToken refreshToken)
        {
            var database = GetMongoDatabase();
            if (database is null) return null;

            var refreshTokenCollection = database.GetCollection<RefreshToken>(nameof(RefreshToken));
            await refreshTokenCollection.InsertOneAsync(refreshToken);

            return refreshToken;
        }

        public async Task<bool> ValidateRefreshToken(string token, string refreshToken, Guid deviceId)
        {
            var database = GetMongoDatabase();
            if (database is null) return false;

            var refreshTokenCollection = database.GetCollection<RefreshToken>(nameof(RefreshToken));
            var refreshTokenRecord = await refreshTokenCollection.FindAsync(record => record.DeviceId == deviceId && record.OriginalToken == token && record.Token == refreshToken);
            if(refreshTokenRecord == null || !refreshTokenRecord.Any()) return false;
            return true;
        }

        public async Task<User?> DeleteRefreshToken(string refreshToken)
        {
            var database = GetMongoDatabase();
            if (database is null) return null;

            var refreshTokenCollection = database.GetCollection<RefreshToken>(nameof(RefreshToken));
            var userCollection = database.GetCollection<User>(nameof(User));
            var refreshTokenRecord =  await (await refreshTokenCollection.FindAsync(record => record.Token == refreshToken)).FirstAsync();
            var user = await userCollection.AsQueryable().FirstOrDefaultAsync(record => record.Id == refreshTokenRecord.UserId);
            await refreshTokenCollection.DeleteOneAsync(record => record.Token == refreshToken);
            return user;
        }
    }
}
