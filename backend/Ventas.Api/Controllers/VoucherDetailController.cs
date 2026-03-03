using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.VoucherDetails.Create;
using Ventas.Application.Entities.VoucherDetails.Delete;
using Ventas.Application.Entities.VoucherDetails.Search;
using Ventas.Application.Entities.VoucherDetails.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherDetailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoucherDetailController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new VoucherDetailSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VoucherDetailCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VoucherDetailUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new VoucherDetailDeleteCommand(id));
            return Ok(result);
        }
    }
}
