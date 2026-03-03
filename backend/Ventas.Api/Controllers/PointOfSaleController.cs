using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.PointOfSales.Create;
using Ventas.Application.Entities.PointOfSales.Delete;
using Ventas.Application.Entities.PointOfSales.Search;
using Ventas.Application.Entities.PointOfSales.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointOfSaleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PointOfSaleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new PointOfSaleSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PointOfSaleCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PointOfSaleUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new PointOfSaleDeleteCommand(id));
            return Ok(result);
        }
    }
}
