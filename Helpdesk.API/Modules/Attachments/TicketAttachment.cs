using Helpdesk.API.Modules.Tickets;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.API.Modules.Attachments
{
    [PrimaryKey(nameof(TicketId),nameof(AttachmentId))]
    public class TicketAttachment
    {

        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public Guid AttachmentId { get; set; }
        public Attachment Attachment { get; set; } = null!;
    }
}
