using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Externas.Prints.BudgetDocument;
using Ventas.Application.Entities.Externas.Prints.DailyBoxDocument;
using Ventas.Application.Entities.Externas.Prints.TicketDocument;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PrintController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ticket/{id}")]
        public async Task<IActionResult> PrintTicket(int id)
        {
            var pdfBytes = await _mediator.Send(new PrintTicketQuery(id));

            if (pdfBytes == null) return NotFound();

            return File(pdfBytes, "application/pdf", $"Ticket_{id}.pdf");
        }

        [HttpGet("budget/{id}")]
        public async Task<IActionResult> PrintBudget(int id)
        {
            var pdfBytes = await _mediator.Send(new BudgetDocumentServiceCommand(id));

            if (pdfBytes == null) return NotFound();

            return File(pdfBytes, "application/pdf", $"Presupuesto_{id}.pdf");
        }

        [HttpGet("dailyBox/{id}")]
        public async Task<IActionResult> PrintDailyBox(int id)
        {
            var pdfBytes = await _mediator.Send(new DailyBoxDocumentServiceCommand(id));

            if (pdfBytes == null) return NotFound();

            return File(pdfBytes, "application/pdf", $"Caja_diaria_{id}.pdf");
        }
    }
}
