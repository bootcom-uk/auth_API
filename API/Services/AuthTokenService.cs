using Extensions.Web;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.Security.Claims;

namespace API.Services
{
    public class AuthTokenService
    {

        private IConfiguration _configuration;
        public AuthTokenService(IConfiguration configuration) { 
        _configuration = configuration; 
        }

        public string CreateToken(User user)
        {
            return new SecurityTokenDescriptor()
                .AddClaim(ClaimTypes.Email, user!.EmailAddress)
                .AddClaim(ClaimTypes.Name, user!.Name)
                .AddClaim(ClaimTypes.NameIdentifier, user.Id.ToString())
                .SetExpiry(DateTime.UtcNow.AddMinutes(15))
                .SetKey(_configuration["JWT_SECRET_KEY"]!)
                .SetAudience(_configuration["JWT_AUDIENCE"]!)
                .SetIssuer(_configuration["JWT_ISSUER"]!)
                .CreateToken();
        }

    }
}
