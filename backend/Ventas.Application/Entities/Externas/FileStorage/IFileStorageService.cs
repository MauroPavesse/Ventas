using Microsoft.AspNetCore.Http;

namespace Ventas.Application.Entities.Externas.FileStorage
{
    public interface IFileStorageService
    {
        Task<string?> UploadFileAsync(IFormFile file, string folderName);
    }
}
