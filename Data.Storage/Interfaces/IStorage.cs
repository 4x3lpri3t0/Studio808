using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Data.Access.Entities;

namespace Data.Storage.Interfaces
{
    public interface IStorage
    {
        ConcurrentDictionary<Guid, string> GetUsersCollection();

        ConcurrentDictionary<Guid, HashSet<Guid>> GetFriendsCollection();

        ConcurrentDictionary<Guid, GameState> GetGameStatesCollection();
    }
}