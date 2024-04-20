using API.Services;
using Extensions.Web;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using MongoDB.Bson;
using System.Diagnostics.Metrics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace API.Controllers
{
    public class RefreshTokenController : BaseController
    {

        private readonly RefreshTokenService _refreshTokenService;

        private readonly AuthTokenService _authTokenService;

        public RefreshTokenController(IDatabaseService databaseService, IConfiguration configuration, RefreshTokenService refreshTokenService, AuthTokenService authTokenService) : base(databaseService, configuration)
        {
            _refreshTokenService = refreshTokenService;
            _authTokenService = authTokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Refresh([FromHeader(Name = "RefreshToken")] string refreshToken, [FromHeader(Name = "DeviceId")] Guid deviceId)
        {

             if (Request.Headers["Authorization"].Count == 0)
            {
                return NotFound();
            }

            var authHeader = Request.Headers["Authorization"][0]!.Replace("Bearer ", "", StringComparison.InvariantCultureIgnoreCase);

            var tokenValidated = await _databaseService.ValidateRefreshToken(authHeader, refreshToken, deviceId);

            if (!tokenValidated)
            {
                return Unauthorized();
            }

            var user = await _databaseService.DeleteRefreshToken(refreshToken);

            if(user is null)
            {
                return NotFound();
            }

            var token = _authTokenService.CreateToken(user);

            var newRefreshToken = await _databaseService.CreateRefreshToken(_refreshTokenService.CreateRefreshToken(user, token, deviceId));

            return Ok(new Dictionary<string, string>()
            {
                { "token", token },
                { "refreshToken", newRefreshToken!.Token },
                { "userId", user.UserId.ToString() }
            });

        }

    }
}
