using System.Net;
using Helpdesk.API.Models;
using Helpdesk.API.Modules.Tickets.Models;
using Microsoft.AspNetCore.Mvc;

namespace Helpdesk.API.Modules.Tickets
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController:ControllerBase
    {
        private readonly TicketService _ticketService;

        public TicketsController(TicketService ticketService)
        {
            _ticketService = ticketService;
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

            return Ok(new CreatedResponse(createdId.Value));
        }
    }
}
