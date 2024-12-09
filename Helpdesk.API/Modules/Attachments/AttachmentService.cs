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
