namespace Helpdesk.API.Modules.Attachments.Models
{
    public record AttachmentResponse(Guid Id, string FileName, string PreSignedUri);

}
