using System.ComponentModel.DataAnnotations;

namespace Helpdesk.API.Modules.Tickets.Models
{
    public class TicketRequest
    {
        [MinLength(5)]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MinLength(5)]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
    }
}
