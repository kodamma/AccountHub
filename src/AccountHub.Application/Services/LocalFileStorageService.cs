using AccountHub.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AccountHub.Application.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IConfiguration config;
        public LocalFileStorageService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<string?> SaveAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if(file == null)
            {
                var path = Path.Combine(config["FileStorage"]!, file!.FileName);
                using var stream = new FileStream(path, FileMode.OpenOrCreate);
                await file.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
                return path;
            }
            return null;
        }
    }
}
