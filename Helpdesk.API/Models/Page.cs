namespace Helpdesk.API.Models
{
    public record Page<TValue>(IEnumerable<TValue> Items, int TotalItems)
    {
    }
}
