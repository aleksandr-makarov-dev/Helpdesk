namespace Helpdesk.API.Modules.Attachments.Models
{
    public record TicketAttachmentRequest(Guid TicketId, Guid AttachmentId);
}
