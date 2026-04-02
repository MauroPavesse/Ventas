using Mapster;
using MediatR;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.DailyBoxes.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Vouchers;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.DailyBoxes.CloseDailyBox
{
    public record CloseDailyBoxServiceCommand(int UserId) : IRequest<DailyBoxOutput>;

    public class CloseDailyBoxServiceHandler : IRequestHandler<CloseDailyBoxServiceCommand, DailyBoxOutput>
    {
        private readonly IDailyBoxRepository _dailyBoxRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CloseDailyBoxServiceHandler(IDailyBoxRepository dailyBoxRepository, IVoucherRepository voucherRepository, IConfigurationRepository configurationRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _dailyBoxRepository = dailyBoxRepository;
            _voucherRepository = voucherRepository;
            _configurationRepository = configurationRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<DailyBoxOutput> Handle(CloseDailyBoxServiceCommand request, CancellationToken cancellationToken)
        {
            var configurations = await _configurationRepository.GetAllAsync();
            var configurationNumber = configurations.First(t => t.Variable == "cajaDiariaNumero");
            configurationNumber.NumericValue++;

            var vouchers = await _voucherRepository.SearchAsync(t => t.Deleted == 0 && t.DailyBoxId == null);

            var dailyBox = await _dailyBoxRepository.CreateAsync(new DailyBox()
            {
                Number = Convert.ToInt32(configurationNumber.NumericValue),
                Amount = vouchers.Sum(t => t.AmountNet + t.AmountVAT),
                Date = DateTime.Now,
                UserId = request.UserId
            });

            await _unitOfWorkRepository.SaveChangesAsync();

            foreach (var voucher in vouchers)
            {
                voucher.DailyBoxId = dailyBox.Id;
                await _voucherRepository.UpdateAsync(voucher);
            }

            await _configurationRepository.UpdateAsync(configurationNumber);

            await _unitOfWorkRepository.SaveChangesAsync();
            return dailyBox.Adapt<DailyBoxOutput>();
        }
    }
}
