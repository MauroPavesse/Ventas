using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Products.Create;
using Ventas.Application.Entities.Products.Delete;
using Ventas.Application.Entities.Products.Search;
using Ventas.Application.Entities.Products.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromQuery] SearchCommand command)
        {
            var result = await _mediator.Send(new ProductSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductUpdateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new ProductDeleteCommand(id));
            return Ok(result);
        }
    }
}
