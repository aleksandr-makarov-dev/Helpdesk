using System.Net;
using Helpdesk.API.Errors;
using Helpdesk.API.Models;
using Helpdesk.API.Modules.Attachments;
using Helpdesk.API.Modules.Attachments.Models;
using Helpdesk.API.Modules.Tickets.Models;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.API.Modules.Tickets
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController:ControllerBase
    {
        private readonly TicketService _ticketService;
        private readonly AttachmentService _attachmentService;

        public TicketsController(TicketService ticketService, AttachmentService attachmentService)
        {
            _ticketService = ticketService;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks([FromQuery] int page = 1)
        {
            var foundPage = await _ticketService.GetTicketsPageAsync(page);

            return Ok(foundPage);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTicketById([FromRoute] Guid id)
        {
            var foundTicket = await _ticketService.GetTicketDetailsByIdAsync(id);

            if (foundTicket.IsFailed)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "404 Not found",
                    Status = (int)HttpStatusCode.NotFound,
                    Detail = foundTicket.Errors.First().Message
                });
            }

            return Ok(foundTicket.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketRequest request)
        {
            var createdId = await _ticketService.CreateTicketAsync(request);

            if (createdId.IsFailed)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "400 Bad request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = createdId.Errors.First().Message
                });
            }

            if (request.Attachments.Any())
            {
                foreach (var attachment in request.Attachments)
                {
                    var attachmentRequest = new TicketAttachmentRequest(createdId.Value, attachment);

                    if (!(await _attachmentService.IsAttachedAsync(attachmentRequest)))
                    {
                        // TODO: check if file was actually attached
                       await _attachmentService.AttachToTicketAsync(attachmentRequest);
                    }
                }
            }

            return Ok(new IdResponse(createdId.Value));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTicket([FromRoute] Guid id, [FromBody] TicketUpdateRequest request)
        {
            var updateResult = await _ticketService.UpdateTicketAsync(id, request);

            if (updateResult.IsFailed)
            {
                if (updateResult.HasError<NotFoundError>())
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "404 Not found",
                        Detail = updateResult.Errors.First().Message
                    });
                }

                return BadRequest(new ProblemDetails()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "400 Bad request",
                    Detail = updateResult.Errors.First().Message
                });
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTicket([FromRoute] Guid id)
        {
            var deleteResult = await _ticketService.DeleteTicketAsync(id);

            if (deleteResult.IsFailed)
            {
                if (deleteResult.HasError<NotFoundError>())
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "404 Not found",
                        Detail = deleteResult.Errors.First().Message
                    });
                }

                return NotFound(new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "400 Bad request",
                    Detail = deleteResult.Errors.First().Message
                });
            }

            return NoContent();
        }
    }
}
