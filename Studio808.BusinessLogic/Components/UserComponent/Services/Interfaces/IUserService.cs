using System.Threading.Tasks;
using Studio808.BusinessLogic.Components.UserComponent.Entities;

namespace Studio808.BusinessLogic.Components.UserComponent.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(string name);
    }
}