using System.Collections.Concurrent;

namespace Studio808.Data.Interfaces
{
    public interface IStorage
    {
        ConcurrentDictionary<string, ConcurrentDictionary<string, string>> GetDatabase();
    }
}
