namespace Ventas.Application.Entities.Externas.Prints.TicketDocument
{
    public interface ITicketDocumentService
    {
        Task<byte[]> PrintTicket(int id);
    }
}
