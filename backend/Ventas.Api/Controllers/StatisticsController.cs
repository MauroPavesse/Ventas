using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Statistics.GetSalesAmount;
using Ventas.Application.Entities.Statistics.TopProducts;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatisticsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sales-amount")]
        public async Task<IActionResult> GetSalesAmount([FromBody] StatisticsGetSalesAmountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("top-products")]
        public async Task<IActionResult> GetTopProducts([FromBody] StatisticsTopProductsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
