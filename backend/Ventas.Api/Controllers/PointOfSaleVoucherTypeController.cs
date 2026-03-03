using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.Create;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.Delete;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.Search;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointOfSaleVoucherTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PointOfSaleVoucherTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new PointOfSaleVoucherTypeSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PointOfSaleVoucherTypeCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PointOfSaleVoucherTypeUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new PointOfSaleVoucherTypeDeleteCommand(id));
            return Ok(result);
        }
    }
}
