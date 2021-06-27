using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Components.UserComponent.Services.Interfaces;
using BusinessLogic.Enums;
using Data.Access.Entities;
using Data.Storage.Interfaces;

namespace BusinessLogic.Components.UserComponent.Services
{
    public class UserService : IUserService
    {
        private readonly IStorage storage;

        public UserService(IStorage storage)
        {
            this.storage = storage;
        }

        public Task<bool> UserExists(Guid userId)
        {
            var usersCollection = this.storage.GetUsersCollection();

            bool found = usersCollection.ContainsKey(userId);

            return Task.FromResult(found);
        }

        public Task<OperationStatus> CreateUser(string name, out User user)
        {
            var usersCollection = this.storage.GetUsersCollection();

            var userId = Guid.NewGuid();
            user = new User(userId, name);

            // Create new user.
            bool successfulAdd = usersCollection.TryAdd(userId, name);
            if (!successfulAdd)
            {
                return Task.FromResult(OperationStatus.AlreadyExists);
            }

            // Create new user game state.
            var gameState = new GameState(0, 0);
            var gameStateCollection = this.storage.GetGameStatesCollection();
            successfulAdd = gameStateCollection.TryAdd(userId, gameState);
            if (!successfulAdd)
            {
                return Task.FromResult(OperationStatus.AlreadyExists);
            }

            return Task.FromResult(OperationStatus.Done);
        }

        public Task<OperationStatus> UpdateGameState(Guid userId, int gamesPlayed, long score, out GameState gameState)
        {
            var gameStatesCollection = this.storage.GetGameStatesCollection();

            gameState = new GameState(gamesPlayed, score);

            GameState expectedCurrentGameState;
            bool successfulGet = gameStatesCollection.TryGetValue(userId, out expectedCurrentGameState);
            if (!successfulGet)
            {
                // Likely due to an incorrect user id.
                return Task.FromResult(OperationStatus.NotFound);
            }

            if (gamesPlayed < expectedCurrentGameState.GamesPlayed)
            {
                // We shouldn't be decreasing the amount of games played on the client.
                return Task.FromResult(OperationStatus.UnknownError);
            }

            bool successfulUpdate = gameStatesCollection.TryUpdate(userId, gameState, expectedCurrentGameState);
            if (!successfulUpdate)
            {
                // Concurrency issue due to race condition.
                // Likely because of to two (or more) similar requests being served at the same time.
                // A retry is expected from client-side to ensure the updated value is the final one.
                return Task.FromResult(OperationStatus.ExpectedStateMismatch);
            }

            return Task.FromResult(OperationStatus.Done);
        }

        public Task<OperationStatus> GetGameState(Guid userId, out GameState gameState)
        {
            var gameStatesCollection = this.storage.GetGameStatesCollection();

            bool successfulGet = gameStatesCollection.TryGetValue(userId, out gameState);
            if (!successfulGet)
            {
                // The user and its state might have been removed.
                return Task.FromResult(OperationStatus.NotFound);
            }

            return Task.FromResult(OperationStatus.Done);
        }

        public Task<OperationStatus> UpdateFriends(Guid userId, List<Guid> newFriendsList, out HashSet<Guid> friends)
        {
            var usersCollection = this.storage.GetUsersCollection();
            var friendsCollection = this.storage.GetFriendsCollection();

            // Keep only users that exist in db.
            friends = newFriendsList
                .Where(friendId => usersCollection.ContainsKey(friendId))
                .ToHashSet();

            // Using indexer because we don't care about old list - we just override it.
            // Also, if the user didn't have a friends list yet, here is where it is initialized.
            friendsCollection[userId] = friends;

            return Task.FromResult(OperationStatus.Done);
        }
    }
}