using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.DailyBoxes.CloseDailyBox;
using Ventas.Application.Entities.DailyBoxes.Create;
using Ventas.Application.Entities.DailyBoxes.Delete;
using Ventas.Application.Entities.DailyBoxes.Search;
using Ventas.Application.Entities.DailyBoxes.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyBoxController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DailyBoxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new DailyBoxSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DailyBoxCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DailyBoxUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DailyBoxDeleteCommand(id));
            return Ok(result);
        }

        [HttpPost("close")]
        public async Task<IActionResult> CloseDailyBox()
        {
            var result = await _mediator.Send(new CloseDailyBoxServiceCommand());
            return Ok(result);
        }
    }
}
