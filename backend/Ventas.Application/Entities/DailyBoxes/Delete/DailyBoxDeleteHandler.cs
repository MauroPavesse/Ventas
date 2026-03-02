using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.DailyBoxes.Delete
{
    public record DailyBoxDeleteCommand(int Id) : IRequest<bool>;

    public class DailyBoxDeleteHandler : IRequestHandler<DailyBoxDeleteCommand, bool>
    {
        private readonly IDailyBoxRepository _dailyBoxRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public DailyBoxDeleteHandler(IDailyBoxRepository dailyBoxRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dailyBoxRepository = dailyBoxRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(DailyBoxDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingDailyBox = await _dailyBoxRepository.GetByIdAsync(request.Id);
            if(existingDailyBox == null)
            {
                throw new KeyNotFoundException($"Caja diaria con ID {request.Id} no encontrada.");
            }
            existingDailyBox.Deleted = 1;
            existingDailyBox.Active = 0;
            var result = await _dailyBoxRepository.UpdateAsync(existingDailyBox);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
