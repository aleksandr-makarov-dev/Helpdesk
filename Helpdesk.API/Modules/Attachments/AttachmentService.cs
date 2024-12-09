using FluentResults;
using Helpdesk.API.Modules.Attachments.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Helpdesk.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.API.Modules.Attachments
{
    public class AttachmentService
    {
        private readonly IStorageService _storageService;
        private readonly ApplicationDbContext _context;

        public AttachmentService(IStorageService storageService, ApplicationDbContext context)
        {
            _storageService = storageService;
            _context = context;
        }

        public async Task<Result<AttachmentResponse>> SaveAttachmentAsync(AttachmentRequest request)
        {
            var preSignedUrlResult = await _storageService.StoreAsync(request.File);

            if (preSignedUrlResult.IsFailed)
            {
                return Result.Fail(new Error(preSignedUrlResult.Errors.First().Message));
            }

            var attachmentToCreate = request.ToAttachment();

            // TODO: validate name
            attachmentToCreate.FileName = request.File.FileName;

            await _context.Attachments.AddAsync(attachmentToCreate);
            var changes = await _context.SaveChangesAsync();

            return changes > 0
                ? Result.Ok(attachmentToCreate.ToAttachmentResponse(preSignedUrlResult.Value))
                : Result.Fail(new Error("Failed to upload file"));
        }

        public async Task<bool> AttachToTicketAsync(TicketAttachmentRequest request)
        {
            var attachmentToCreate = request.ToTicketAttachment();
            await _context.TicketAttachments.AddAsync(attachmentToCreate);
            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> IsAttachedAsync(TicketAttachmentRequest request)
        {
            return await _context.TicketAttachments.AnyAsync(ta =>
                ta.AttachmentId == request.AttachmentId && ta.TicketId == request.TicketId);
        }

        public async Task<Result<List<AttachmentResponse>>> GetAttachmentsByTicketIdAsync(Guid ticketId)
        {
            try
            {
                var foundAttachments = await _context
                    .TicketAttachments
                    .Where(ta => ta.TicketId == ticketId)
                    .Include(ta => ta.Attachment)
                    .Select(ta=>ta.Attachment)
                    .ToListAsync();

                var attachmentResponses = new List<AttachmentResponse>();

                foreach (var foundAttachment in foundAttachments)
                {
                    // TODO: check that pre signed url was created
                    var preSignedUrl = await _storageService.RetrieveAsync(foundAttachment.FileName);

                    attachmentResponses.Add(foundAttachment.ToAttachmentResponse(preSignedUrl));
                }

                return Result.Ok(attachmentResponses);
            }
            catch (Exception e)
            {
                return Result.Fail(new Error(e.Message));
            }
        }

        public async Task<IEnumerable<Attachment>> GetAllAttachmentsAsync()
        {
            return await _context.Attachments.ToListAsync();
        }

        public async Task<bool> DeleteAttachmentById(Guid attachmentId)
        {
            var ticketAttachment = await _context.TicketAttachments.FirstOrDefaultAsync(ta=>ta.AttachmentId == attachmentId);
            if (ticketAttachment is not null)
            {
                _context.TicketAttachments.Remove(ticketAttachment);
            }

            var attachment = await _context.Attachments.FirstOrDefaultAsync(a=>a.Id == attachmentId);

            if (attachment is not null)
            {
                _context.Attachments.Remove(attachment);
            }

            int changes = await _context.SaveChangesAsync();

            return changes > 0;
        }

        public Result ValidateAttachment(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                return Result.Fail(new Error("The file is not attached"));
            }

            if (!FileValidator.IsFileExtensionAllowed(file, [".pdf", ".doc", ".docx", ".png", ".jpg"]))
            {
                return Result.Fail(new Error("Invalid file type. Please upload only PDF, DOC, DOCX, PNG or JPG file"));
            }

            if (!FileValidator.IsFileWithinLimit(file, 5_000_000))
            {
                return Result.Fail(new Error("File size exceeds the maximum allowed size (5 MB)."));
            }

            return Result.Ok();
        }
    }
}
