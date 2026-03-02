using Mapster;
using MediatR;
using Ventas.Application.Entities.DailyBoxes.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.DailyBoxes.Update
{
    public record DailyBoxUpdateCommand(int Id, int Number, decimal Amount) : IRequest<DailyBoxOutput>;

    public class DailyBoxUpdateHandler : IRequestHandler<DailyBoxUpdateCommand, DailyBoxOutput>
    {
        private readonly IDailyBoxRepository _dailyBoxRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public DailyBoxUpdateHandler(IDailyBoxRepository dailyBoxRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dailyBoxRepository = dailyBoxRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<DailyBoxOutput> Handle(DailyBoxUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingDailyBox = await _dailyBoxRepository.GetByIdAsync(request.Id);
            if (existingDailyBox == null)
            {
                throw new KeyNotFoundException($"Caja diaria con Id {request.Id} no encontrada.");
            }
            existingDailyBox.Number = request.Number;
            existingDailyBox.Amount = request.Amount;
            var updatedDailyBox = await _dailyBoxRepository.UpdateAsync(existingDailyBox);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedDailyBox.Adapt<DailyBoxOutput>();
        }
    }
}
