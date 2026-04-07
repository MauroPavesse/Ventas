using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Externas.Prints.TicketDocument;
using Ventas.Application.Entities.Vouchers.CloseSale;
using Ventas.Application.Entities.Vouchers.ConvertToInvoice;
using Ventas.Application.Entities.Vouchers.Create;
using Ventas.Application.Entities.Vouchers.Delete;
using Ventas.Application.Entities.Vouchers.Search;
using Ventas.Application.Entities.Vouchers.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoucherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCommand command)
        {
            var result = await _mediator.Send(new VoucherSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VoucherUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new VoucherDeleteCommand(id));
            return Ok(result);
        }

        [HttpPost("close-sale")]
        public async Task<IActionResult> CloseSale([FromBody] CloseSaleServiceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("convert-to-invoice")]
        public async Task<IActionResult> ConvertToInvoice([FromBody] ConvertToInvoiceServiceCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
