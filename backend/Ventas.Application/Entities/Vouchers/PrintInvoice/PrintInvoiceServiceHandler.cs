using MediatR;
using Ventas.Application.Entities.Externas.GeneratePdf;

namespace Ventas.Application.Entities.Vouchers.PrintInvoice
{
    public record PrintInvoiceQuery(int VoucherId, bool IsTicket) : IRequest<byte[]>;

    public class PrintInvoiceHandler : IRequestHandler<PrintInvoiceQuery, byte[]>
    {
        private readonly IGenerateInvoicePdfService _pdfService;

        public PrintInvoiceHandler(IGenerateInvoicePdfService pdfService)
        {
            _pdfService = pdfService;
        }

        public async Task<byte[]> Handle(PrintInvoiceQuery request, CancellationToken cancellationToken)
        {
            return await _pdfService.GenerateInvoicePdf(request.VoucherId, request.IsTicket);
        }
    }
}
