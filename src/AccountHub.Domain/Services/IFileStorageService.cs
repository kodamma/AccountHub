using Microsoft.AspNetCore.Http;

namespace AccountHub.Domain.Services
{
    public interface IFileStorageService
    {
        ValueTask<string> SaveAsync(IFormFile? file, CancellationToken cancellationToken);
    }
}
