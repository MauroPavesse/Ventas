using MediatR;

namespace Ventas.Application.Entities.Externas.Prints.BudgetDocument
{
    public record BudgetDocumentServiceCommand(int VoucherId) : IRequest<byte[]>;

    public class BudgetDocumentServiceHandler : IRequestHandler<BudgetDocumentServiceCommand, byte[]>
    {
        private readonly IBudgetDocumentService _budgetDocumentService;

        public BudgetDocumentServiceHandler(IBudgetDocumentService budgetDocumentService)
        {
            _budgetDocumentService = budgetDocumentService;
        }

        public async Task<byte[]> Handle(BudgetDocumentServiceCommand request, CancellationToken cancellationToken)
        {
            return await _budgetDocumentService.PrintBudget(request.VoucherId);
        }
    }
}
