using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Studio808.Data.Interfaces
{
    public interface IStorage
    {
        ConcurrentDictionary<Guid, string> GetUsersCollection();

        ConcurrentDictionary<Guid, HashSet<Guid>> GetUserFriendsCollection();
    }
}