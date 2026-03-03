using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.PaymentMethods.Create;
using Ventas.Application.Entities.PaymentMethods.Delete;
using Ventas.Application.Entities.PaymentMethods.Search;
using Ventas.Application.Entities.PaymentMethods.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new PaymentMethodSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentMethodCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PaymentMethodUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new PaymentMethodDeleteCommand(id));
            return Ok(result);
        }
    }
}
