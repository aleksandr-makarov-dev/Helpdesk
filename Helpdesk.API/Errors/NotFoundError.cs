using FluentResults;

namespace Helpdesk.API.Errors
{
    public class NotFoundError(string message) : Error(message);
}
