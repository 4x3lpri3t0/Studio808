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
            bool success = usersCollection.TryAdd(userId, name);
            if (!success)
            {
                return Task.FromResult(OperationStatus.AlreadyExists);
            }

            // Create new user game state.
            var gameState = new GameState(0, 0);
            var gameStateCollection = this.storage.GetGameStatesCollection();
            success = gameStateCollection.TryAdd(userId, gameState);
            if (!success)
            {
                return Task.FromResult(OperationStatus.AlreadyExists);
            }

            return Task.FromResult(OperationStatus.Done);
        }

        public Task<OperationStatus> UpdateGameState(Guid userId, int gamesPlayed, long score, out GameState gameState)
        {
            var gameStatesCollection = this.storage.GetGameStatesCollection();

            gameState = new GameState(gamesPlayed, score);

            GameState currentGameState;
            bool successfulGet = gameStatesCollection.TryGetValue(userId, out currentGameState);
            if (!successfulGet)
            {
                // Likely due to an incorrect user id.
                return Task.FromResult(OperationStatus.NotFound);
            }

            if (gamesPlayed < currentGameState.GamesPlayed ||
                score < currentGameState.Score)
            {
                // We shouldn't be decreasing the amount of games played on the client.
                // Since the score is a highscore, it shouldn't decrease either.
                return Task.FromResult(OperationStatus.ExpectedStateMismatch);
            }

            bool updated = gameStatesCollection.TryUpdate(userId, gameState, currentGameState);
            if (!updated)
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

            bool exists = gameStatesCollection.TryGetValue(userId, out gameState);
            if (!exists)
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

            // Keep only users that exist in db and don't allow users to befriend themselves.
            friends = newFriendsList
                .Where(friendId => usersCollection.ContainsKey(friendId) && userId != friendId)
                .ToHashSet();

            // Using indexer because we don't care about old list - we just override it.
            // Also, if the user didn't have a friends list yet, here is where it is initialized.
            friendsCollection[userId] = friends;

            return Task.FromResult(OperationStatus.Done);
        }

        public Task<OperationStatus> GetFriendScores(Guid userId, out List<FriendScore> friendScores)
        {
            var friendsCollection = this.storage.GetFriendsCollection();
            var usersCollection = this.storage.GetUsersCollection();
            var gameStatesCollection = this.storage.GetGameStatesCollection();

            // We assume that the user has under a couple dozen friends.
            // For this to scale and support thousands of friends, it would be
            // recommended to use pagination (and potentially other techniques).
            friendScores = new List<FriendScore>();

            // First we get all current user friends.
            // If the user doesn't have any, we return the empty friends list initialized earlier.
            HashSet<Guid> friendIds;
            bool exists = friendsCollection.TryGetValue(userId, out friendIds);
            if (!exists)
            {
                return Task.FromResult(OperationStatus.Done);
            }

            // Now we need to get their names and highscores.
            foreach (var friendId in friendIds)
            {
                string friendName;
                exists = usersCollection.TryGetValue(friendId, out friendName);
                if (!exists)
                {
                    // Friend is not a registered user. Ignore.
                    Console.WriteLine($"Username could not be found for friend with id {friendId}");
                    continue;
                }

                GameState friendGameState;
                exists = gameStatesCollection.TryGetValue(friendId, out friendGameState);
                if (!exists)
                {
                    // Friend might have been removed between requests. Ignore.
                    Console.WriteLine($"GameState could not be found for friend with id {friendId}");
                    continue;
                }

                friendScores.Add(new FriendScore(friendId, friendName, friendGameState.Score));
            }

            return Task.FromResult(OperationStatus.Done);
        }

        public List<User> DebugGetAllUsers()
        {
            // Temp solution for DEBUG purposes.
            return this.storage
                .GetUsersCollection()
                .Select(kv => new User(kv.Key, kv.Value))
                .ToList();
        }
    }
}