using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Ventas.Application.Entities.Vouchers.DTOs;

namespace Ventas.Infrastructure.Persistence.Reporting
{
    public class InvoiceDocument : IDocument
    {
        public record InvoiceDocumentCommand(
            VoucherOutput Voucher,
            string BusinessName,
            string BusinessCuit,
            string BusinessTaxCondition,
            string BusinessAddress,
            string CustomerTaxCondition,
            byte[]? QrCodeImage
        );

        private readonly InvoiceDocumentCommand _model;

        public InvoiceDocument(InvoiceDocumentCommand model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

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
                        col.Item().Text(_model.BusinessName).FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text(_model.BusinessAddress);
                        col.Item().Text(_model.BusinessTaxCondition);
                    });

                    // Info del Comprobante (Derecha)
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"CUIT: {_model.BusinessCuit}").SemiBold();
                        col.Item().Text($"Fecha: {_model.Voucher.DateCreation:dd/MM/yyyy}");
                        col.Item().Text($"Hora: {_model.Voucher.DateCreation:HH:mm}");
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
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Encabezado de tabla
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Precio");
                            header.Cell().Element(CellStyle).AlignRight().Text("Total");

                            static IContainer CellStyle(IContainer container) =>
                                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        });

                        // Filas
                        foreach (var item in _model.Voucher.VoucherDetails)
                        {
                            table.Cell().PaddingVertical(5).Text(item.ProductName);
                            table.Cell().PaddingVertical(5).AlignRight().Text(item.Quantity.ToString());
                            table.Cell().PaddingVertical(5).AlignRight().Text($"${item.PriceUnit:N2}");
                            table.Cell().PaddingVertical(5).AlignRight().Text($"${item.AmountFinal:N2}");
                        }
                    });

                    // Total
                    col.Item().AlignRight().PaddingTop(10)
                        .Text($"TOTAL: ${_model.Voucher.VoucherDetails.Sum(t => t.AmountFinal):N2}")
                        .FontSize(14).Bold();
                });

                // --- PIE DE PÁGINA (Aquí va el QR) ---
                page.Footer().Column(col =>
                {
                    // QR de AFIP
                    if (_model.QrCodeImage != null)
                    {
                        col.Item().AlignCenter().Width(2, Unit.Centimetre).Image(_model.QrCodeImage);
                    }

                    col.Item().AlignCenter().PaddingTop(5).Text(x =>
                    {
                        x.Span("Gracias por su compra!").FontSize(12).Italic();
                    });

                    col.Item().AlignCenter().Text(x => {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });
        }
    }
}