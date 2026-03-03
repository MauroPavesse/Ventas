using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.VoucherPayments.Create;
using Ventas.Application.Entities.VoucherPayments.Delete;
using Ventas.Application.Entities.VoucherPayments.Search;
using Ventas.Application.Entities.VoucherPayments.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherPaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoucherPaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new VoucherPaymentSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherPaymentCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VoucherPaymentUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new VoucherPaymentDeleteCommand(id));
            return Ok(result);
        }
    }
}
