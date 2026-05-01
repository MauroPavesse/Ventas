using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Externas.Afip;

namespace Ventas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AfipController : ControllerBase
    {
        private readonly IAfipService afipService;

        public AfipController(IAfipService afipService)
        {
            this.afipService = afipService;
        }

        public class AfipTestRequest
        {
            public string CertificadoPath { get; set; } = "";
            public string Clave { get; set; } = "";
        }

        [HttpPost("test-connection")]
        public async Task<IActionResult> TestConnection([FromBody] AfipTestRequest req)
        {
            try
            {
                var success = afipService.IsCertificatePasswordCorrect(req.CertificadoPath, req.Clave);

                if (success)
                    return Ok(new { message = "Conexión exitosa" });

                return BadRequest(new { message = "AFIP rechazó las credenciales" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
