using System.Collections.Concurrent;
using Studio808.Data.Interfaces;

namespace Studio808.Data
{
    public class Storage : IStorage
    {
        // Note: Temporarily use a concurrent dictionary to simulate a document collection.
        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> UserFriendsCollection { get; set; }

        public Storage()
        {
            UserFriendsCollection = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
        }

        public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> GetDatabase()
        {
            return this.UserFriendsCollection;
        }
    }
}