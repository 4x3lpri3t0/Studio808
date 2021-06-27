using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Api.Requests;
using BusinessLogic.Components.UserComponent.Dtos;
using BusinessLogic.Components.UserComponent.Services.Interfaces;
using BusinessLogic.Enums;
using BusinessLogic.Helpers;
using Data.Access.Entities;
using System.Collections.Generic;
using System.Linq;

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

            // Success.
            return Created(uri: "/", value: user.ToDto<UserDto>());
        }

        [HttpPut]
        [Route("{userId}/state")]
        public async Task<IActionResult> SaveGameState(Guid userId, [FromBody] SaveGameStateRequest request)
        {
            // Validations.
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            // Store game state.
            GameState gameState;
            var operationStatus = await userService
                .UpdateGameState(userId, request.GamesPlayed, request.Score, out gameState)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            // Success.
            return Ok(value: gameState.ToDto<GameStateDto>());
        }

        [HttpGet]
        [Route("{userId}/state")]
        public async Task<IActionResult> LoadGameState(Guid userId)
        {
            // Validations.
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            // Retrieve game state.
            GameState gameState;
            var operationStatus = await userService
                .GetGameState(userId, out gameState)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            // Success.
            return Ok(value: gameState.ToDto<GameStateDto>());
        }

        [HttpPut]
        [Route("{userId}/friends")]
        public async Task<IActionResult> UpdateFriends(Guid userId, [FromBody] UpdateFriendsRequest request)
        {
            // Validations.
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            // Store new friends for user.
            HashSet<Guid> friends;
            var operationStatus = await userService
                .UpdateFriends(userId, request.Friends, out friends)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            // Success.
            var resultDto = new FriendsDto() { Friends = friends };
            return Ok(value: resultDto);
        }

        [HttpGet]
        [Route("{userId}/friends")]
        public async Task<IActionResult> GetFriends(Guid userId)
        {
            // Validations.
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            // Get user friends with their corresponding scores.
            List<FriendScore> friendScores;
            var operationStatus = await userService
                .GetFriendScores(userId, out friendScores) // AXEL TODO: Parse toDto
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            // Success.
            var friendScoreDtoList = friendScores.Select(x => x.ToDto<FriendScoreDto>()).ToList();
            var resultDto = new FriendScoresDto() { FriendScores = friendScoreDtoList };
            return Ok(value: resultDto);
        }

        [HttpGet]
        public async Task<IActionResult> DebugGetAllUsers()
        {
            // TODO
            return Ok();
        }

        private async Task<bool> UserExists(Guid userId)
        {
            return await userService
                            .UserExists(userId)
                            .ConfigureAwait(false);
        }
    }
}