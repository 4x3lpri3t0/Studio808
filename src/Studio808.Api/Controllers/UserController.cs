using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Studio808.Api.Requests;
using Studio808.BusinessLogic.Components.UserComponent.Dtos;
using Studio808.BusinessLogic.Components.UserComponent.Services.Interfaces;
using Studio808.BusinessLogic.Helpers;

namespace Studio808.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            // Create and store the new user.
            var user = await userService.CreateUser(request.name).ConfigureAwait(false);

            return Created(uri: "/", value: user.ToDto<UserDto>());
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