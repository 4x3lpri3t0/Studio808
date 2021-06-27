using System;

namespace Data.Access.Entities
{
    public class FriendScore : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public long Score { get; set; }

        public FriendScore(Guid id, string name, long score)
        {
            Id = id;
            Name = name;
            Score = score;
        }
    }
}
