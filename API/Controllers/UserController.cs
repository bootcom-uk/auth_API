using Amazon.Auth.AccessControlPolicy;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Security.Claims;
using System.Security.Permissions;

namespace API.Controllers
{

    public class UserController : BaseController
    {
        public UserController(IDatabaseService databaseService, IConfiguration configuration) : base(databaseService, configuration)
        {
        }

        [HttpGet]
        public async Task<ActionResult<Models.User?>> GetUser()
        {
            var id = ObjectId.Parse(User.Claims.Where(record => record.Type == ClaimTypes.NameIdentifier).First().Value);
            return await _databaseService.GetUserById(id);
        }

        [HttpGet]
        [Route("id")]
        public ActionResult<ObjectId> GetId()
        {
            var item = User.Claims.Where(record => record.Type == ClaimTypes.NameIdentifier).First().Value;
            var userId = ObjectId.Parse(item);
            return Ok(userId);
        }

        [HttpGet]
        [Route("name")]
        public ActionResult<String> GetName()
        {
            return Ok(User.Claims.Where(record => record.Type == ClaimTypes.Name).First().Value);
        }

    }
}
