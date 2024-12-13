using FluentResults;
using Helpdesk.API.Domain;
using Helpdesk.API.Errors;
using Helpdesk.API.Models;
using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Tickets.Models;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.API.Modules.Tickets
{
    public class TicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly AttachmentService _attachmentService;

        public TicketService(ApplicationDbContext context, AttachmentService attachmentService)
        {
            _context = context;
            _attachmentService = attachmentService;
        }

        public async Task<Page<TicketResponse>> GetTicketsPageAsync(int page = 1, int limit = 10, Guid? userId = null)
        {
            var query = _context.Tickets.AsQueryable();

            if (userId is not null)
            {
                query = query.Where(t => t.RequesterId == userId);
            }

            IEnumerable<TicketResponse> foundTickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(t=>new TicketResponse(t.Id,t.Title,t.Priority,t.Status,t.CreatedAt,t.RequesterId))
                .ToListAsync();

            var count = await query.CountAsync();

            return new Page<TicketResponse>(foundTickets,count);
        }

        public async Task<Result<TicketDetailsResponse>> GetTicketDetailsByIdAsync(Guid id)
        {
            var foundTicket = await _context
                .Tickets
                .FirstOrDefaultAsync(t => t.Id == id);

            if (foundTicket is null)
            {
                return Result.Fail(new Error($"Ticket {id} does not exist"));
            }

            var foundAttachments = await _attachmentService.GetAttachmentsByTicketIdAsync(foundTicket.Id);

            if (foundAttachments.IsFailed)
            {
                return Result.Fail(new Error("Failed to retrieve attachments"));
            }

            return Result.Ok(foundTicket.ToTicketDetailsResponse(foundAttachments.Value));
        }

        public async Task<Result<Guid>> CreateTicketAsync(TicketRequest request,Guid requesterId)
        {
            var ticketToCreate = request.ToTicket();
            ticketToCreate.CreatedAt = DateTime.UtcNow;
            ticketToCreate.RequesterId = requesterId;

            await _context.Tickets.AddAsync(ticketToCreate);
            var changes = await _context.SaveChangesAsync();

            return changes > 0 ? Result.Ok(ticketToCreate.Id) : Result.Fail(new Error("Failed to create ticket"));
        }

        public async Task<Result> UpdateTicketAsync(Guid ticketId,TicketUpdateRequest request)
        {
            var foundTicket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (foundTicket is null)
            {
                return Result.Fail(new NotFoundError($"Ticket {ticketId} does not exist"));
            }

            if (foundTicket.Status == TicketStatus.Closed)
            {
                return Result.Fail(new Error("Closed ticket cannot be updated"));
            }

            if (request.TicketPriority.HasValue)
            {
                foundTicket.Priority = request.TicketPriority.Value;
            }

            if (request.TicketStatus.HasValue)
            {
                foundTicket.Status = request.TicketStatus.Value;
            }

            var changes = await _context.SaveChangesAsync();

            return changes > 0 ? Result.Ok() : Result.Fail(new Error("Failed to update ticket"));
        }

        public async Task<Result> DeleteTicketAsync(Guid id)
        {
            var foundTicket = await _context.Tickets.Include(t=>t.TicketAttachments).FirstOrDefaultAsync(t => t.Id == id);

            if (foundTicket is null)
            {
                return Result.Fail(new NotFoundError($"Ticket {id} does not exist"));
            }

            if (foundTicket.Status == TicketStatus.Closed)
            {
                return Result.Fail(new Error("Closed ticket cannot be deleted"));
            }

            foreach (var attachment in foundTicket.TicketAttachments.ToList())
            {
                
                // TODO: can optimize and don't fetch ticket attachment twice
                await _attachmentService.DeleteAttachmentById(attachment.AttachmentId);
            }

            // TODO: clear files in minio (LATER)
            _context.Tickets.Remove(foundTicket);
            int changes = await _context.SaveChangesAsync();

            return changes > 0 ? Result.Ok() : Result.Fail(new Error("Failed to delete ticket"));
        }

        public async Task<bool> IsTicketOwnerAsync(Guid ticketId, Guid userId)
        {
            return await _context.Tickets.AnyAsync(t => t.Id == ticketId && t.RequesterId == userId);
        }
    }
}
