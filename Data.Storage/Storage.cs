using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Data.Access.Entities;
using Data.Storage.Interfaces;

namespace Data.Storage
{
    /// <summary>
    /// Class used to simulate calls to an actual database.
    /// The purpose of each ConcurrentDictionary is to represent a database collection (NoSQL) or table (SQL).
    /// The first key of the dictionary is used as a document PartitionKey (NoSQL) or a table PrimaryKey (SQL).
    /// By using a ConcurrentDictionary instead of a common Dictionary (hashtable), we can observe and think about concurrency.
    /// </summary>
    public class Storage : IStorage
    {
        // TODO/Note: Temporarily use these data structures to simulate persistent storage.
        private ConcurrentDictionary<Guid, string> UsersCollection { get; set; }
        private ConcurrentDictionary<Guid, GameState> GameStatesCollection { get; set; }
        private ConcurrentDictionary<Guid, HashSet<Guid>> FriendsCollection { get; set; }

        public Storage()
        {
            UsersCollection = new ConcurrentDictionary<Guid, string>();
            GameStatesCollection = new ConcurrentDictionary<Guid, GameState>();
            FriendsCollection = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public ConcurrentDictionary<Guid, string> GetUsersCollection()
        {
            return this.UsersCollection;
        }

        public ConcurrentDictionary<Guid, GameState> GetGameStatesCollection()
        {
            return this.GameStatesCollection;
        }

        public ConcurrentDictionary<Guid, HashSet<Guid>> GetFriendsCollection()
        {
            return this.FriendsCollection;
        }
    }
}