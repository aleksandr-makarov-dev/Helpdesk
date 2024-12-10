using System.ComponentModel.DataAnnotations;
using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Users;

namespace Helpdesk.API.Modules.Tickets
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Pending;
        public TicketPriority Priority { get; set; } = TicketPriority.Low;
        public DateTime CreatedAt { get; set; }
        public Guid RequesterId { get; set; }
        public User Requester { get; set; } = null!;
        public ICollection<TicketAttachment> TicketAttachments { get; set; } = new List<TicketAttachment>();
    }

    public enum TicketStatus
    {
        Pending,
        Working,
        Closed,
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High
    }
}
