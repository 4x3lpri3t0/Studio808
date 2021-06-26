using System;
using Studio808.DataAccess.Entities;

namespace Studio808.BusinessLogic.Components.UserComponent.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
