using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Api;
using Api.Requests;
using BusinessLogic.Components.UserComponent.Dtos;
using Xunit;
using static Tests.Helpers.TestHelper;

namespace ApiTests
{
    public class Tests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public Tests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_User_WhenMissingUsername_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            string url = "user";
            var request = new { };

            // Act (POST with missing username) + Assert
            await PostAsync<object>(client, url, request, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_User_WhenEmptyUsername_BadRequest()
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
        public async Task Put_GameState_WhenNegativeScore_BadRequest()
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
        public async Task Put_GameState_WhenMissingGamesPlayed_BadRequest()
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
            string userUrl = "user";
            var userRequest = new CreateUserRequest("Alice");

            // Act
            var userResponse = await PostAsync<UserDto>(client, userUrl, userRequest, HttpStatusCode.Created);

            // Arrange
            string gameStateUrl = $"user/{userResponse.Id}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);

            // Act + Assert
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.OK);
        }

        [Fact]
        public async Task Put_GameState_WhenNonExistentUser_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            var nonExistentUserId = Guid.NewGuid();
            string url = $"user/{nonExistentUserId}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);

            // Act + Assert
            await PutAsync(client, url, gameStateRequest, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_GameState_WhenGamesPlayedIsLessThanPrevious_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            string userUrl = "user";
            var userRequest = new CreateUserRequest("Alice");

            // Act
            var userResponse = await PostAsync<UserDto>(client, userUrl, userRequest, HttpStatusCode.Created);

            // Arrange
            string gameStateUrl = $"user/{userResponse.Id}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);

            // Act + Assert
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.OK);

            // Arrange (decrease number of games played)
            gameStateRequest = new SaveGameStateRequest(9, 300);

            // Act + Assert
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.BadRequest);
        }
    }
}