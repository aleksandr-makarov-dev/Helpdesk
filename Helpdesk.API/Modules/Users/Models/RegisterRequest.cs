using System.ComponentModel.DataAnnotations;

namespace Helpdesk.API.Modules.Users.Models
{
    public class RegisterRequest
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
