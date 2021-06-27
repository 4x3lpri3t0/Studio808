using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Requests
{
    public class UpdateFriendsRequest
    {
        [Required]
        public List<Guid> Friends { get; set; }

        public UpdateFriendsRequest(List<Guid> friends)
        {
            Friends = friends;
        }
    }
}