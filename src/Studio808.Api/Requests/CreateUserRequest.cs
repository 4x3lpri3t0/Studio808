using System.ComponentModel.DataAnnotations;

namespace Studio808.Api.Requests
{
    public class CreateUserRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string name { get; set; }

        public CreateUserRequest(string name)
        {
            this.name = name;
        }
    }
}
