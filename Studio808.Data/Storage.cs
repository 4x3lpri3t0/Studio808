using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Studio808.Data.Interfaces;

namespace Studio808.Data
{
    public class Storage : IStorage
    {
        // TODO/Note: Temporarily use these data structures to simulate persistent storage.
        private ConcurrentDictionary<Guid, string> UsersCollection { get; set; }
        private ConcurrentDictionary<Guid, HashSet<Guid>> UserFriendsCollection { get; set; }

        public Storage()
        {
            UsersCollection = new ConcurrentDictionary<Guid, string>();
            UserFriendsCollection = new ConcurrentDictionary<Guid, HashSet<Guid>>();
        }

        public ConcurrentDictionary<Guid, string> GetUsersCollection()
        {
            return this.UsersCollection;
        }

        public ConcurrentDictionary<Guid, HashSet<Guid>> GetUserFriendsCollection()
        {
            return this.UserFriendsCollection;
        }
    }
}