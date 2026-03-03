using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.TaxConditions.Search;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxConditionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaxConditionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new TaxConditionSearchCommand(command));
            return Ok(result);
        }
    }
}
