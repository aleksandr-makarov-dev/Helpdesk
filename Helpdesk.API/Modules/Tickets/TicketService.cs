using FluentResults;
using Helpdesk.API.Domain;
using Helpdesk.API.Models;
using Helpdesk.API.Modules.Tickets.Models;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.API.Modules.Tickets
{
    public class TicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Page<TicketResponse>> GetTicketsPageAsync(int page = 1, int limit = 10)
        {
            IEnumerable<TicketResponse> foundTickets = await _context.Tickets
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(t=>new TicketResponse(t.Id,t.Title,t.Priority,t.Status,t.CreatedAt))
                .ToListAsync();

            var count = await _context.Tickets.CountAsync();

            return new Page<TicketResponse>(foundTickets,count);
        }

        public async Task<Result<TicketDetailsResponse>> GetTicketDetailsByIdAsync(Guid id)
        {
            var foundTicket = await _context
                .Tickets
                .FirstOrDefaultAsync(t => t.Id == id);

            return foundTicket is null ? Result.Fail(new Error($"Ticket {id} does not exist")) : Result.Ok(foundTicket.ToTicketDetailsResponse());
        }

        public async Task<Result<Guid>> CreateTicketAsync(TicketRequest request)
        {
            var ticketToCreate = request.ToTicket();
            ticketToCreate.CreatedAt = DateTime.UtcNow;

            await _context.Tickets.AddAsync(ticketToCreate);
            var changes = await _context.SaveChangesAsync();

            return changes > 0 ? Result.Ok(ticketToCreate.Id) : Result.Fail(new Error("Failed to create ticket"));
        }
    }
}
