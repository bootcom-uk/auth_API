using API.Services;
using Communication;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;

namespace API.Controllers
{
    public class AuthenticationController : BaseController
    {

        private readonly IEmailProviderService _emailProviderService;

        private readonly RefreshTokenService _refreshTokenService;

        private readonly AuthTokenService _authTokenService;

        public AuthenticationController(IDatabaseService databaseService, IConfiguration configuration, IEmailProviderService emailProviderService, RefreshTokenService refreshTokenService, AuthTokenService authTokenService) : base(databaseService, configuration)
        {
            _emailProviderService = emailProviderService;
            _refreshTokenService = refreshTokenService;
            _authTokenService = authTokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("{quickAccessCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Dictionary<string, string>>> AuthenticateQuickAccessCode([FromRoute] string quickAccessCode, [FromHeader] Guid deviceId)
        {
            var user = await _databaseService.ValidateQuickAccessCode(quickAccessCode, deviceId);
            if (user is null)
            {
                return NotFound();
            }

            var token = _authTokenService.CreateToken(user);

            var refreshToken = await _databaseService.CreateRefreshToken(_refreshTokenService.CreateRefreshToken(user, token, deviceId));

            return Ok(new Dictionary<string, string>()
            {
                { "token", token },
                { "refreshToken", refreshToken!.Token },
                { "userId", user.UserId.ToString() }
            });

        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RequestToken([FromHeader(Name = "EmailAddress")] string emailAddress, [FromHeader(Name = "DeviceId")] Guid deviceId)
        {
            var user = await _databaseService.GetUserByEmailAddress(emailAddress);

            // User is returned as null meaning they don't exist in the database, so create them
            if (user is null)
            {
                await _databaseService.CreateUser(emailAddress);
            }

            // If this user doesn't already exist in the database or they're 
            // not activated we need to send a welcome email
            if (user is null || !user.EmailAddressConfirmed)
            {
                await _emailProviderService.Send(_configuration["EXCHANGE_EMAIL_NO_REPLY_FROM"]!, "Welcome To BootCom", _configuration["EMAIL_TEMPLATE_WELCOME"]!, new Dictionary<string, string>(), new List<Exchange365EmailRecipient>()
            {
                { new() { Contact = emailAddress, Name = emailAddress} }
            });
                return Unauthorized();
            }

            var accessCodeRequest = await _databaseService.SendAuthToken(emailAddress, deviceId);

            if(accessCodeRequest is null)
            {
                return Unauthorized();
            }

            await _emailProviderService.Send(_configuration["EXCHANGE_EMAIL_NO_REPLY_FROM"]!, "Login To BootCom", _configuration["EMAIL_TEMPLATE_LOGIN"]!, new Dictionary<string, string>()
            {
                { "{{BASE_URL}}", _configuration["WEBSITE_BOOTCOM_LOGIN_BASE_URL"]!},
                { "{{ACCESS_CODE}}", accessCodeRequest!.QuickAccessCode},
                { "{{LOGIN_CODE}}", accessCodeRequest!.LoginCode.ToString()}
            }, new List<Exchange365EmailRecipient>()
            {
                { new() { Contact = emailAddress, Name = emailAddress} }
            });


            return NoContent();
        }

    }
}
