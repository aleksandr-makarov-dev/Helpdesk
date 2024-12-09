using System.ComponentModel.DataAnnotations;

namespace Helpdesk.API.Modules.Attachments.Models
{
    public class AttachmentRequest
    {
        public required IFormFile File { get; set; }
        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
