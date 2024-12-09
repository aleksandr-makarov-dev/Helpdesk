using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Attachments.Models;
using Helpdesk.API.Modules.Tickets.Models;

namespace Helpdesk.API.Modules.Tickets
{
    public static class TicketMapper
    {
        public static TicketDetailsResponse ToTicketDetailsResponse(this Ticket t, List<AttachmentResponse> attachments)
        {

            return new TicketDetailsResponse(
                t.Id, 
                t.Title, 
                t.Description, 
                t.Priority, 
                t.Status, 
                t.CreatedAt,
                attachments
                );
        }

        public static Ticket ToTicket(this TicketRequest r)
        {
            return new Ticket
            {
                Title = r.Title,
                Description = r.Description,
                Status = TicketStatus.Pending,
                Priority = TicketPriority.Low
            };
        }

    }
}
