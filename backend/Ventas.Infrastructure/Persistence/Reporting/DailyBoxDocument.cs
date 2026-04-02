using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Ventas.Application.Entities.VoucherPayments.DTOs;
using Ventas.Application.Entities.Vouchers.DTOs;

namespace Ventas.Infrastructure.Persistence.Reporting
{
    public class DailyBoxDocument : IDocument
    {
        public record DailyBoxDocumentCommand(
            string DailyBoxDescription,
            DateTime DailyBoxDate,
            string PointOfSaleDescription,
            List<VoucherOutput> Vouchers,
            List<VoucherPaymentOutput> Payments
        );

        private readonly DailyBoxDocumentCommand _model;

        public DailyBoxDocument(DailyBoxDocumentCommand model)
        {
            _model = model;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                // --- CABECERA ---
                page.Header().Row(row =>
                {
                    // Info de la Empresa (Izquierda)
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(_model.DailyBoxDescription).FontSize(20).SemiBold();
                        col.Item().Text($"Fecha: {_model.DailyBoxDate.ToString("dd/MM/yyyy HH:mm")}");
                        col.Item().Text($"Punto de venta: {_model.PointOfSaleDescription}");
                    });
                });

                // --- CONTENIDO ---
                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                        });

                        // Encabezado de tabla
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Comprobante");
                            header.Cell().Element(CellStyle).AlignRight().Text("Importe");

                            static IContainer CellStyle(IContainer container) =>
                                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        });

                        // Filas
                        foreach (var item in _model.Vouchers)
                        {
                            table.Cell().PaddingVertical(5).Text(item.Description);
                            table.Cell().PaddingVertical(5).AlignRight().Text($"${item.AmountTotal:N2}");
                        }
                    });

                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                        });

                        // Encabezado de tabla
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Forma de pago");
                            header.Cell().Element(CellStyle).AlignRight().Text("Importe");

                            static IContainer CellStyle(IContainer container) =>
                                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        });

                        // Filas
                        foreach (var item in _model.Payments)
                        {
                            table.Cell().PaddingVertical(5).Text(item.PaymentMethod?.Name ?? "");
                            table.Cell().PaddingVertical(5).AlignRight().Text($"${item.Amount:N2}");
                        }
                    });

                    // Total
                    col.Item().AlignRight().PaddingTop(10)
                        .Text($"TOTAL: ${_model.Vouchers.Sum(t => t.AmountTotal):N2}")
                        .FontSize(14).Bold();
                });
            });
        }
    }
}
