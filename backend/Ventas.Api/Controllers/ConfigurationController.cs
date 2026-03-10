using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Configurations.Search;
using Ventas.Application.Entities.Configurations.Update;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] List<string> command)
        {
            var result = await _mediator.Send(new ConfigurationSearchCommand(command));
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateConfigurationsBatchCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
