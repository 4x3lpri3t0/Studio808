using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Studio808.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] string name)
        {
            // TODO
            return Ok();
        }

        [HttpPut]
        [Route("{userid}/state")]
        public async Task<IActionResult> SaveGameState(Guid userid)
        {
            // TODO
            return Ok();
        }

        [HttpGet]
        [Route("{userid}/state")]
        public async Task<IActionResult> LoadGameState(Guid userid)
        {
            // TODO
            return Ok();
        }

        [HttpPut]
        [Route("{userid}/friends")]
        public async Task<IActionResult> UpdateFriends(Guid userid)
        {
            // TODO
            return Ok();
        }

        [HttpGet]
        [Route("{userid}/friends")]
        public async Task<IActionResult> GetFriends(Guid userid)
        {
            // TODO
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> DebugGetAllUsers(Guid userid)
        {
            // TODO
            return Ok();
        }
    }
}
