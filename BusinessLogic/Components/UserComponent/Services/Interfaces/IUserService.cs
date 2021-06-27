using System;
using System.Threading.Tasks;
using BusinessLogic.Enums;
using Data.Access.Entities;

namespace BusinessLogic.Components.UserComponent.Services.Interfaces
{
    public interface IUserService
    {
        Task<OperationStatus> CreateUser(string name, out User user);

        Task<OperationStatus> UpdateGameState(Guid userId, int gamesPlayed, long score, out GameState gameState);
    }
}