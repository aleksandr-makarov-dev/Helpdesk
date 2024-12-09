namespace Helpdesk.API.Modules.Tickets.Models
{
    public class TicketUpdateRequest
    {
        public TicketStatus? TicketStatus { get; set; }
        public TicketPriority? TicketPriority { get; set; }
    }
}
