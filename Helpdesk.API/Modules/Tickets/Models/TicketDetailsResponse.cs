using Helpdesk.API.Modules.Attachments.Models;

namespace Helpdesk.API.Modules.Tickets.Models
{
    public record TicketDetailsResponse(
        Guid Id,
        string Title,
        string Description,
        TicketPriority Priority,
        TicketStatus Status,
        DateTime CreatedAt,
        List<AttachmentResponse> Attachments
        );
}
