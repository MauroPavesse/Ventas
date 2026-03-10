using Microsoft.AspNetCore.Http;
using Ventas.Application.Entities.Externas.FileStorage;

namespace Ventas.Infrastructure.Persistence.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _basePath = "/app/wwwroot/uploads";

        public async Task<string?> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            var targetFolder = Path.Combine(_basePath, folderName);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}
