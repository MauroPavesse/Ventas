using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Externas.FileStorage;

namespace Ventas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly IFileStorageService _fileService;

        public UploadsController(IFileStorageService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("certificate")]
        public async Task<IActionResult> UploadCertificate(IFormFile file)
        {
            // Puedes obtener el subdominio del Header Host o de un JWT
            var subdominio = Request.Host.Host.Split('.')[0];

            var resultPath = await _fileService.UploadFileAsync(file, $"{subdominio}/certificates");

            return Ok(new { url = resultPath });
        }
    }
}
