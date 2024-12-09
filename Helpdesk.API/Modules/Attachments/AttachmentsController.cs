﻿using System.Net;
using Helpdesk.API.Modules.Attachments.Models;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.API.Modules.Attachments
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AttachmentsController:ControllerBase
    {
        private readonly AttachmentService _attachmentService;

        public AttachmentsController(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachment([FromForm] AttachmentRequest request)
        {
            var valid = _attachmentService.ValidateAttachment(request.File);

            if (valid.IsFailed)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "400 Bad Request",
                    Detail = valid.Errors.First().Message
                });
            }

            var attachmentResult  = await _attachmentService.SaveAttachmentAsync(request);

            if (attachmentResult.IsFailed)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "400 Bad Request",
                    Detail = attachmentResult.Errors.First().Message
                });
            }

            return Ok(attachmentResult.Value);
        }
    }
}
