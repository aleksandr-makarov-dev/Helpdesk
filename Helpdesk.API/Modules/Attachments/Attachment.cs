using System.ComponentModel.DataAnnotations;
using Helpdesk.API.Modules.Tickets;

namespace Helpdesk.API.Modules.Attachments
{
    public class Attachment
    {
        [Key]
        public Guid Id { get; set; }

        [MinLength(1)]
        [MaxLength(1000)]
        public string FileName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<TicketAttachment> TicketAttachments = new List<TicketAttachment>();
    }
}
