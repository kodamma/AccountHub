using Microsoft.AspNetCore.Http;

namespace AccountHub.Domain.Services
{
    public interface IFileStorageService
    {
        Task<string?> SaveAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
