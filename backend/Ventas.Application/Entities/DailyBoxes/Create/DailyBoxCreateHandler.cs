using Mapster;
using MediatR;
using Ventas.Application.Entities.DailyBoxes.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.DailyBoxes.Create
{
    public record DailyBoxCreateCommand(int Number, decimal Amount, int UserId) : IRequest<DailyBoxOutput>;

    public class DailyBoxCreateHandler : IRequestHandler<DailyBoxCreateCommand, DailyBoxOutput>
    {
        private readonly IDailyBoxRepository _dailyBoxRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public DailyBoxCreateHandler(IDailyBoxRepository dailyBoxRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dailyBoxRepository = dailyBoxRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<DailyBoxOutput> Handle(DailyBoxCreateCommand request, CancellationToken cancellationToken)
        {
            var dailyBox = await _dailyBoxRepository.CreateAsync(request.Adapt<DailyBox>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return dailyBox.Adapt<DailyBoxOutput>();
        }
    }
}
