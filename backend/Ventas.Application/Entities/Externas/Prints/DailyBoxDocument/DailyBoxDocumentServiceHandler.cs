using MediatR;

namespace Ventas.Application.Entities.Externas.Prints.DailyBoxDocument
{
    public record DailyBoxDocumentServiceCommand(int DailyBoxId) : IRequest<byte[]>;

    public class DailyBoxDocumentServiceHandler : IRequestHandler<DailyBoxDocumentServiceCommand, byte[]>
    {
        private readonly IDailyBoxDocumentService _dailyBoxDocumentService;

        public DailyBoxDocumentServiceHandler(IDailyBoxDocumentService dailyBoxDocumentService)
        {
            _dailyBoxDocumentService = dailyBoxDocumentService;
        }

        public async Task<byte[]> Handle(DailyBoxDocumentServiceCommand request, CancellationToken cancellationToken)
        {
            return await _dailyBoxDocumentService.PrintDailyBox(request.DailyBoxId);
        }
    }
}
