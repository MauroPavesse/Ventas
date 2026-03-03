using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.VoucherTypes.Search;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoucherTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new VoucherTypeSearchCommand(command));
            return Ok(result);
        }
    }
}
