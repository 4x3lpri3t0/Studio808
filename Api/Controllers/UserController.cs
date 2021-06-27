using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Requests;
using BusinessLogic.Components.UserComponent.Dtos;
using BusinessLogic.Components.UserComponent.Services.Interfaces;
using BusinessLogic.Enums;
using BusinessLogic.Helpers;
using Data.Access.Entities;
using Microsoft.AspNetCore.Mvc;

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
        [Route("{userId}/state")]
        public async Task<IActionResult> SaveGameState(Guid userId, [FromBody] SaveGameStateRequest request)
        {
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

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
        [Route("{userId}/state")]
        public async Task<IActionResult> LoadGameState(Guid userId)
        {
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            GameState gameState;
            var operationStatus = await userService
                .GetGameState(userId, out gameState)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            return Ok(value: gameState.ToDto<GameStateDto>());
        }

        [HttpPut]
        [Route("{userId}/friends")]
        public async Task<IActionResult> UpdateFriends(Guid userId, [FromBody] UpdateFriendsRequest request)
        {
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            HashSet<Guid> friends;
            var operationStatus = await userService
                .UpdateFriends(userId, request.Friends, out friends)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            var resultDto = new FriendsDto() { Friends = friends };
            return Ok(value: resultDto);
        }

        [HttpGet]
        [Route("{userId}/friends")]
        public async Task<IActionResult> GetFriends(Guid userId)
        {
            if (!await UserExists(userId))
            {
                return NotFound($"User with id {userId} does not exist.");
            }

            List<FriendScore> friendScores;
            var operationStatus = await userService
                .GetFriendScores(userId, out friendScores)
                .ConfigureAwait(false);

            if (operationStatus != OperationStatus.Done)
            {
                return BadRequest(operationStatus.ToString());
            }

            var friendScoreDtoList = friendScores.Select(x => x.ToDto<FriendScoreDto>()).ToList();
            var resultDto = new FriendScoresDto() { FriendScores = friendScoreDtoList };
            return Ok(value: resultDto);
        }

        [HttpGet]
        public IActionResult DebugGetAllUsers()
        {
            var users = userService.DebugGetAllUsers();
            return Ok(value: users);
        }

        private async Task<bool> UserExists(Guid userId)
        {
            return await userService
                            .UserExists(userId)
                            .ConfigureAwait(false);
        }
    }
}