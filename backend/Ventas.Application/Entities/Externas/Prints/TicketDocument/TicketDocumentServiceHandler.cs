using MediatR;

namespace Ventas.Application.Entities.Externas.Prints.TicketDocument
{
    public record PrintTicketQuery(int VoucherId) : IRequest<byte[]>;

    public class TicketDocumentServiceHandler : IRequestHandler<PrintTicketQuery, byte[]>
    {
        private readonly ITicketDocumentService _pdfService;

        public TicketDocumentServiceHandler(ITicketDocumentService pdfService)
        {
            _pdfService = pdfService;
        }

        public async Task<byte[]> Handle(PrintTicketQuery request, CancellationToken cancellationToken)
        {
            return await _pdfService.PrintTicket(request.VoucherId);
        }
    }
}
