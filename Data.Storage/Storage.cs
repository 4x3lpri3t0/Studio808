using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Data.Access.Entities;
using Data.Storage.Interfaces;

namespace Data.Storage
{
    /// <summary>
    /// Used to simulate calls to an actual database.
    /// The purpose of each ConcurrentDictionary is to represent a database collection/document (NoSQL) or table (SQL).
    /// The first key of the dictionary is used as a PartitionKey (NoSQL) or PrimaryKey (SQL).
    /// By using a ConcurrentDictionary instead of a common Dictionary (hashtable), we can observe and think about concurrency.
    /// </summary>
    public class Storage : IStorage
    {
        // TODO/Note: Temporarily use these data structures to simulate persistent storage.
        private ConcurrentDictionary<Guid, string> UserCollection { get; set; }
        private ConcurrentDictionary<Guid, GameState> UserGameStateCollection { get; set; }
        private ConcurrentDictionary<Guid, HashSet<Guid>> UserFriendsCollection { get; set; }

        public Storage()
        {
            UserCollection = new ConcurrentDictionary<Guid, string>();
            UserGameStateCollection = new ConcurrentDictionary<Guid, GameState>();
            UserFriendsCollection = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public ConcurrentDictionary<Guid, string> GetUsersCollection()
        {
            return this.UserCollection;
        }

        public ConcurrentDictionary<Guid, GameState> GetUserGameStateCollection()
        {
            return this.UserGameStateCollection;
        }

        public ConcurrentDictionary<Guid, HashSet<Guid>> GetUserFriendsCollection()
        {
            return this.UserFriendsCollection;
        }
    }
}