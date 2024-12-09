namespace Helpdesk.API.Modules.Tickets.Models
{
    public record TicketResponse(
        Guid Id,
        string Title,
        TicketPriority Priority,
        TicketStatus Status,
        DateTime CreatedAt
        );
}
