using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Api.Requests;
using BusinessLogic.Components.UserComponent.Dtos;
using Xunit;
using static Tests.Helpers.TestHelper;
using System.Collections.Generic;

namespace Api.Tests
{
    public class Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public Tests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_User_MissingUsername_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            string url = "user";
            var request = new { };

            // Act (POST with missing username) + Assert
            await PostAsync<object>(client, url, request, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_User_EmptyUsername_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            string url = "user";
            string username = string.Empty;
            var request = new CreateUserRequest(username);

            // Act (POST empty username) + Assert
            await PostAsync<UserDto>(client, url, request, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_Users()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            string url = "user";
            var request1 = new CreateUserRequest("Alice");
            var request2 = new CreateUserRequest("Bob");

            // Act (POST two different users)
            var response1 = await PostAsync<UserDto>(client, url, request1, HttpStatusCode.Created);
            var response2 = await PostAsync<UserDto>(client, url, request2, HttpStatusCode.Created);

            // Assert (Check that the created users match their intended names)
            Assert.Equal(request1.Name, response1.Name);
            Assert.Equal(request2.Name, response2.Name);
        }

        [Fact]
        public async Task Put_GameState_NegativeScore_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var userId = Guid.NewGuid();
            string url = $"user/{userId}/state";
            var request = new SaveGameStateRequest(10, -100);

            // Act (PUT with missing score) + Assert
            await PutAsync(client, url, request, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_GameState_MissingGamesPlayed_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var userId = Guid.NewGuid();
            string url = $"user/{userId}/state";
            var request = new { score = 42 }; // Force missing games played

            // Act (PUT with missing games played) + Assert
            await PutAsync(client, url, request, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_GameState()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string gameStateUrl = $"user/{user.Id}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);

            // Act + Assert
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.OK);
        }

        [Fact]
        public async Task Put_GameState_NonExistentUser_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var nonExistentUserId = Guid.NewGuid();
            string url = $"user/{nonExistentUserId}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);

            // Act + Assert
            await PutAsync(client, url, gameStateRequest, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_GameState_GamesPlayedIsLessThanPrevious_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string gameStateUrl = $"user/{user.Id}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.OK);
            gameStateRequest = new SaveGameStateRequest(9, 300); // Decrease number of games played

            // Act + Assert
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_GameState_NonExistentUser_NotFound()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            Guid nonExistentUserId = Guid.NewGuid();
            string url = $"user/{nonExistentUserId}/state";

            // Act
            var userResponse = await GetAsync(client, url, HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_GameState_NewUser()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/state";

            // Act
            var gameState = await GetAsync<GameStateDto>(client, url, HttpStatusCode.OK);

            // Assert
            Assert.Equal(0, gameState.GamesPlayed);
            Assert.Equal(0, gameState.Score);
        }

        [Fact]
        public async Task Get_GameState_AfterUpdate()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/state";
            int gamesPlayedNewValue = 42;
            long scoreNewValue = 1337133713371337L;
            var gameStateRequest = new SaveGameStateRequest(gamesPlayedNewValue, scoreNewValue);
            await PutAsync(client, url, gameStateRequest, HttpStatusCode.OK);

            // Act
            var gameState = await GetAsync<GameStateDto>(client, url, HttpStatusCode.OK);

            // Assert
            Assert.Equal(gamesPlayedNewValue, gameState.GamesPlayed);
            Assert.Equal(scoreNewValue, gameState.Score);
        }

        [Fact]
        public async Task Put_Friends()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            UserDto friend = await CreateUser(client);
            var friends = new List<Guid>() { friend.Id };
            var request = new UpdateFriendsRequest(friends);

            // Act
            var friendsResult = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(friendsResult);
            Assert.NotNull(friendsResult.Friends);
            Assert.NotEmpty(friendsResult.Friends);
            Assert.Equal(friends.Count, friendsResult.Friends.Count);
        }

        [Fact]
        public async Task Put_Friends_NonExistent()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            Guid nonExistentFriendId = Guid.NewGuid();
            var friends = new List<Guid>() { nonExistentFriendId };
            var request = new UpdateFriendsRequest(friends);

            // Act
            var friendsResult = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(friendsResult);
            Assert.NotNull(friendsResult.Friends);
            Assert.Empty(friendsResult.Friends); // 200 but don't store unexistent id.
        }

        [Fact]
        public async Task Put_Friends_DuplicateIds()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            UserDto friend = await CreateUser(client);
            var friends = new List<Guid>() { friend.Id, friend.Id }; // Duplicate ids.
            var request = new UpdateFriendsRequest(friends);

            // Act
            var friendsResult = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(friendsResult);
            Assert.NotNull(friendsResult.Friends);
            Assert.NotEmpty(friendsResult.Friends);
            Assert.Equal(1, friendsResult.Friends.Count); // Only one is stored.
        }
    }
}