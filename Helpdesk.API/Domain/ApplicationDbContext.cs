using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Tickets;
using Helpdesk.API.Modules.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.API.Domain
{
    public class ApplicationDbContext:IdentityDbContext<User,IdentityRole<Guid>,Guid>
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
    }
}
