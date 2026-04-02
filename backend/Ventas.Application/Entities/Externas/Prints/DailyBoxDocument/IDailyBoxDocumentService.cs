namespace Ventas.Application.Entities.Externas.Prints.DailyBoxDocument
{
    public interface IDailyBoxDocumentService
    {
        Task<byte[]> PrintDailyBox(int id);
    }
}
