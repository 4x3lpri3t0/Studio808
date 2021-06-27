using System;

namespace Data.Access.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public User(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
