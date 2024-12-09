using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Tickets;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.API.Domain
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
    }
}
