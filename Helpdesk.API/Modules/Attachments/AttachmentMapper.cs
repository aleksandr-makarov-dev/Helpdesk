using Helpdesk.API.Modules.Attachments.Models;

namespace Helpdesk.API.Modules.Attachments
{
    public static class AttachmentMapper
    {
        public static Attachment ToAttachment(this AttachmentRequest r)
        {
            return new Attachment
            {
                Description = r.Description,
            };
        }

        public static AttachmentResponse ToAttachmentResponse(this Attachment a, string preSignedUrl)
        {
            return new AttachmentResponse(a.Id, a.FileName, preSignedUrl);
        }
    }
}
