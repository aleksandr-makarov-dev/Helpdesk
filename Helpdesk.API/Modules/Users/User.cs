using Helpdesk.API.Modules.Tickets;
using Microsoft.AspNetCore.Identity;

namespace Helpdesk.API.Modules.Users
{
    public class User:IdentityUser<Guid>
    {
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
