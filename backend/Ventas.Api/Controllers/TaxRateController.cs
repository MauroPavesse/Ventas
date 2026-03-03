using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.TaxRates.Search;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxRateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaxRateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new TaxRateSearchCommand(command));
            return Ok(result);
        }
    }
}
