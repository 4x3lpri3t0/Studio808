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
using System.Linq;

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
        public async Task Put_GameState_DecreasingGamesPlayed_BadRequest()
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
        public async Task Put_GameState_DecreasingHighscore_BadRequest()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string gameStateUrl = $"user/{user.Id}/state";
            var gameStateRequest = new SaveGameStateRequest(10, 300);
            await PutAsync(client, gameStateUrl, gameStateRequest, HttpStatusCode.OK);
            gameStateRequest = new SaveGameStateRequest(11, 299); // Decrease highscore

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
            var result = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Friends);
            Assert.NotEmpty(result.Friends);
            Assert.Equal(friends.Count, result.Friends.Count);
        }

        [Fact]
        public async Task Put_Friends_BefriendYourself()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            var friends = new List<Guid>() { user.Id }; // Self
            var request = new UpdateFriendsRequest(friends);

            // Act
            var result = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Friends);
            Assert.Empty(result.Friends); // 200 but don't store user as own friend
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
            var result = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Friends);
            Assert.Empty(result.Friends); // 200 but don't store unexistent id
        }

        [Fact]
        public async Task Put_Friends_DuplicateIds()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            UserDto friend = await CreateUser(client, "Bob");
            var friends = new List<Guid>() { friend.Id, friend.Id }; // Duplicate ids
            var request = new UpdateFriendsRequest(friends);

            // Act
            var result = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Friends);
            Assert.NotEmpty(result.Friends);
            Assert.Single(result.Friends); // Only one is stored
        }

        [Fact]
        public async Task Get_Friends_Empty()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";

            // Act
            var result = await GetAsync<FriendScoresDto>(client, url, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.FriendScores);
            Assert.Empty(result.FriendScores); // 200 but no friends
        }

        [Fact]
        public async Task Get_Friends_Two()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            UserDto friend1 = await CreateUser(client, "Bob");
            UserDto friend2 = await CreateUser(client, "Charlie");

            long scoreFriend1 = 100;
            long scoreFriend2 = 200;
            await SaveGameState(client, friend1.Id, 1, scoreFriend1);
            await SaveGameState(client, friend2.Id, 2, scoreFriend2);

            var friends = new List<Guid>() { friend1.Id, friend2.Id };
            var request = new UpdateFriendsRequest(friends);
            var friendsResult = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Act
            var result = await GetAsync<FriendScoresDto>(client, url, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(friendsResult);
            var storedFriends = result.FriendScores;
            Assert.NotNull(storedFriends);
            Assert.NotEmpty(storedFriends);
            Assert.Equal(2, storedFriends.Count); // Both were stored

            var storedFriend1 = storedFriends.Single(x => x.Id == friend1.Id);
            var storedFriend2 = storedFriends.Single(x => x.Id == friend2.Id);

            Assert.NotNull(storedFriend1);
            Assert.NotNull(storedFriend2);
            Assert.Equal(friend1.Name, storedFriend1.Name);
            Assert.Equal(friend2.Name, storedFriend2.Name);
            Assert.Equal(scoreFriend1, storedFriend1.Score);
            Assert.Equal(scoreFriend2, storedFriend2.Score);
        }

        [Fact]
        public async Task Get_Friends_Duplicates()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            UserDto user = await CreateUser(client);
            string url = $"user/{user.Id}/friends";
            UserDto friend = await CreateUser(client, "Bob");

            long scoreFriend1 = 42;
            await SaveGameState(client, friend.Id, 1, scoreFriend1);

            var friends = new List<Guid>() { friend.Id, friend.Id }; // Duplicate
            var request = new UpdateFriendsRequest(friends);
            var friendsResult = await PutAsync<FriendsDto>(client, url, request, HttpStatusCode.OK);

            // Act
            var result = await GetAsync<FriendScoresDto>(client, url, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(friendsResult);
            var storedFriends = result.FriendScores;
            Assert.NotNull(storedFriends);
            Assert.NotEmpty(storedFriends);
            Assert.Single(storedFriends); // Stored only once

            var storedFriend1 = storedFriends.Single(x => x.Id == friend.Id);

            Assert.NotNull(storedFriend1);
            Assert.Equal(friend.Name, storedFriend1.Name);
            Assert.Equal(scoreFriend1, storedFriend1.Score);
        }

        [Fact]
        public async Task Debug_Get_Users()
        {
            // Arrange
            HttpClient client = _factory.CreateClient();
            string url = "user";
            UserDto user1 = await CreateUser(client, "Alice");
            UserDto user2 = await CreateUser(client, "Bob");
            UserDto user3 = await CreateUser(client, "Charlie");

            // Act
            var result = await GetAsync<List<UserDto>>(client, url, HttpStatusCode.OK);

            // Assert
            Assert.NotNull(result);

            var storedUser1 = result.Single(x => x.Id == user1.Id);
            var storedUser2 = result.Single(x => x.Id == user2.Id);
            var storedUser3 = result.Single(x => x.Id == user3.Id);

            Assert.Equal(user1.Name, storedUser1.Name);
            Assert.Equal(user2.Name, storedUser2.Name);
            Assert.Equal(user3.Name, storedUser3.Name);
        }
    }
}