namespace Ventas.Application.Entities.Externas.GeneratePdf
{
    public interface IGenerateInvoicePdfService
    {
        Task<byte[]> GenerateInvoicePdf(int id, bool isTicket = false);
    }
}
