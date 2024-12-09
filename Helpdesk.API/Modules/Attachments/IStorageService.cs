using FluentResults;

namespace Helpdesk.API.Modules.Attachments
{
    public interface IStorageService
    {
        Task<Result<string>> StoreAsync(IFormFile file);
        Task<string?> RetrieveAsync(string fileName);
    }
}
