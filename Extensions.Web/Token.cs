using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Extensions.Web
{
    public static class Token
    {
        public static SecurityTokenDescriptor Initialize(this SecurityTokenDescriptor securityTokenDescriptor)
        {
            securityTokenDescriptor.Subject = new ClaimsIdentity();
            return securityTokenDescriptor;
        }

        public static SecurityTokenDescriptor AddClaim(this SecurityTokenDescriptor securityTokenDescriptor, string claimType, string claimValue)
        {
            if (securityTokenDescriptor.Subject == null)
            {
                securityTokenDescriptor.Initialize();
            }

            securityTokenDescriptor!.Subject!.AddClaim(new Claim(claimType, claimValue));
            return securityTokenDescriptor;
        }

        public static SecurityTokenDescriptor SetExpiry(this SecurityTokenDescriptor securityTokenDescriptor, DateTime expiry)
        {
            securityTokenDescriptor.Expires = expiry;
            return securityTokenDescriptor;
        }

        public static SecurityTokenDescriptor SetKey(this SecurityTokenDescriptor securityTokenDescriptor, string key)
        {
            securityTokenDescriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), "HS256");
            return securityTokenDescriptor;
        }

        public static SecurityTokenDescriptor SetIssuer(this SecurityTokenDescriptor securityTokenDescriptor, string issuer)
        {
            securityTokenDescriptor.Issuer = issuer;
            return securityTokenDescriptor;
        }

        public static SecurityTokenDescriptor SetAudience(this SecurityTokenDescriptor securityTokenDescriptor, string audience)
        {
            securityTokenDescriptor.Audience = audience;
            return securityTokenDescriptor;
        }

        public static string CreateToken(this SecurityTokenDescriptor securityTokenDescriptor)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    }
}
