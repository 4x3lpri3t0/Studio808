using System;
using System.Threading.Tasks;
using Studio808.BusinessLogic.Components.UserComponent.Entities;
using Studio808.BusinessLogic.Components.UserComponent.Services.Interfaces;
using Studio808.Data.Interfaces;

namespace Studio808.BusinessLogic.Components.UserComponent.Services
{
    public class UserService : IUserService
    {
        private readonly IStorage storage;

        public UserService(IStorage storage)
        {
            this.storage = storage;
        }

        public Task<User> CreateUser(string name)
        {
            Guid userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = name
            };

            // Simulate database call.
            var usersCollection = this.storage.GetUsersCollection();
            usersCollection.TryAdd(userId, name);

            return Task.FromResult(user);
        }
    }
}