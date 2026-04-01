namespace Ventas.Application.Entities.Externas.Prints.BudgetDocument
{
    public interface IBudgetDocumentService
    {
        Task<byte[]> PrintBudget(int id);
    }
}
