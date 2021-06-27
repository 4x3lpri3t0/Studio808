using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Requests;
using BusinessLogic.Components.UserComponent.Dtos;
using BusinessLogic.Components.UserComponent.Services.Interfaces;
using BusinessLogic.Enums;
using BusinessLogic.Helpers;
using Data.Access.Entities;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
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
            // Create and store new user.
            User user;
            var operationStatus = await userService
                .CreateUser(request.Name, out user)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            return Created(uri: "/", value: user.ToDto<UserDto>());
        }

        [HttpPut]
        [Route("{userid}/state")]
        public async Task<IActionResult> SaveGameState(Guid userId, [FromBody] SaveGameStateRequest request)
        {
            // Store game state.
            GameState gameState;
            var operationStatus = await userService
                .UpdateGameState(userId, request.GamesPlayed, request.Score, out gameState)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            return Ok(value: gameState.ToDto<GameStateDto>());
        }

        [HttpGet]
        [Route("{userid}/state")]
        public async Task<IActionResult> LoadGameState(Guid userId)
        {
            // Retrieve game state.
            GameState gameState;
            var operationStatus = await userService
                .GetGameState(userId, out gameState)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return NotFound(operationStatus.ToString());
            }

            return Ok(value: gameState.ToDto<GameStateDto>());
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