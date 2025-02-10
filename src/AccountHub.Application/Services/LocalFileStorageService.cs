using AccountHub.Domain.Services;
using Kodamma.Common.Base.Utilities;
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

        public async ValueTask<string> SaveAsync(IFormFile? file, CancellationToken cancellationToken)
        {
            if(file != null)
            {
                string newFileName = RandomStringGenerator.Generate(file!.FileName.Length);
                var extension = Path.GetExtension(file!.FileName);
                var path = Path.Combine(config["FileStorage"]!, (newFileName + extension));
                using var stream = new FileStream(path, FileMode.OpenOrCreate);
                await file.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);
                return path;
            }
            return string.Empty;
        }
    }
}
