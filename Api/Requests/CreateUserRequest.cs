using System.ComponentModel.DataAnnotations;

namespace Api.Requests
{
    public class CreateUserRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public CreateUserRequest(string name)
        {
            this.Name = name;
        }
    }
}
