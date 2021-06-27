using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Enums;
using Data.Access.Entities;

namespace BusinessLogic.Components.UserComponent.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UserExists(Guid userId);

        Task<OperationStatus> CreateUser(string name, out User user);

        Task<OperationStatus> UpdateGameState(Guid userId, int gamesPlayed, long score, out GameState gameState);

        Task<OperationStatus> GetGameState(Guid userId, out GameState gameState);

        Task<OperationStatus> UpdateFriends(Guid userId, List<Guid> newFriendsList, out HashSet<Guid> friends);

        Task<OperationStatus> GetFriendScores(Guid userId, out List<FriendScore> friends);
    }
}