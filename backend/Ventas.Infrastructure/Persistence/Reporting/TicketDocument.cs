using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Ventas.Infrastructure.Persistence.Reporting
{
    public class TicketDocument : IDocument
    {
        private readonly InvoiceDocument.InvoiceDocumentCommand _model;

        public TicketDocument(InvoiceDocument.InvoiceDocumentCommand model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                // Configuración para ticketera de 80mm
                page.ContinuousSize(8, Unit.Centimetre);
                page.Margin(0.3f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.CourierNew)); // Fuente monoespaciada queda mejor en tickets

                page.Content().Column(col =>
                {
                    // CABECERA CENTRADA
                    col.Item().AlignCenter().Text(_model.BusinessName).FontSize(12).Bold();
                    col.Item().AlignCenter().Text(_model.BusinessAddress).FontSize(8);
                    col.Item().AlignCenter().Text($"CUIT: {_model.BusinessCuit}").FontSize(8);
                    col.Item().AlignCenter().Text(_model.BusinessTaxCondition).FontSize(8);

                    col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

                    // INFO COMPROBANTE
                    col.Item().Text($"Fecha: {_model.Voucher.DateCreation:dd/MM/yy HH:mm}");
                    col.Item().Text($"Comp: {_model.Voucher.Description}").SemiBold();

                    col.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);

                    // TABLA DE PRODUCTOS (Simplificada)
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Producto
                            columns.RelativeColumn(1); // Cant
                            columns.RelativeColumn(2); // Total
                        });

                        foreach (var item in _model.Voucher.VoucherDetails)
                        {
                            table.Cell().Text(item.ProductName);
                            table.Cell().AlignRight().Text($"{item.Quantity}x");
                            table.Cell().AlignRight().Text($"${item.AmountFinal:N2}");
                        }
                    });

                    col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);

                    // TOTAL
                    col.Item().AlignRight().Text(x =>
                    {
                        x.Span("TOTAL: ").FontSize(12).Bold();
                        x.Span($"${_model.Voucher.VoucherDetails.Sum(t => t.AmountFinal):N2}").FontSize(12).Bold();
                    });

                    // QR DE AFIP (Fundamental en el ticket)
                    if (_model.QrCodeImage != null)
                    {
                        col.Item().PaddingTop(10).AlignCenter().Width(3, Unit.Centimetre).Image(_model.QrCodeImage);
                    }

                    col.Item().PaddingTop(5).AlignCenter().Text("CAE: " + _model.Voucher.CAE).FontSize(8);
                    col.Item().AlignCenter().Text("--- GRACIAS POR SU COMPRA ---").FontSize(8);
                });
            });
        }
    }
}
