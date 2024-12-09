using Helpdesk.API.Modules.Tickets;

namespace Helpdesk.API.Domain
{
    public static class Seeder
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Tickets.Any())
                {
                    var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Server Down",
                    Description = "The production server is down.",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.High,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Bug in Login Page",
                    Description = "Users cannot log in due to a JavaScript error.",
                    Status = TicketStatus.Working,
                    Priority = TicketPriority.High,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Feature Request: Dark Mode",
                    Description = "Add support for dark mode in the web app.",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.Medium,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Database Optimization",
                    Description = "Optimize the database for faster queries.",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.Medium,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Update Privacy Policy",
                    Description = "Revise and publish the new privacy policy.",
                    Status = TicketStatus.Working,
                    Priority = TicketPriority.Low,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Mobile App Crash",
                    Description = "The mobile app crashes when opening the settings page.",
                    Status = TicketStatus.Working,
                    Priority = TicketPriority.High,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Performance Issue",
                    Description = "The web app takes too long to load on certain pages.",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.High,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Add New Analytics Dashboard",
                    Description = "Create a new analytics dashboard for marketing insights.",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.Medium,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Email Notifications",
                    Description = "Email notifications are not being sent to users.",
                    Status = TicketStatus.Closed,
                    Priority = TicketPriority.High,
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = Guid.NewGuid(),
                    Title = "Update API Documentation",
                    Description = "The API documentation needs to be updated with the latest changes.",
                    Status = TicketStatus.Pending,
                    Priority = TicketPriority.Low,
                    CreatedAt = DateTime.UtcNow
                }
            };

                    await context.Tickets.AddRangeAsync(tickets);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
