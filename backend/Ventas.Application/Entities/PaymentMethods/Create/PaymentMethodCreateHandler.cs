using Mapster;
using MediatR;
using Ventas.Application.Entities.PaymentMethods.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PaymentMethods.Create
{
    public record PaymentMethodCreateCommand(string Name, decimal DescountPercentage, decimal IncreasePercentage, string Color) : IRequest<PaymentMethodOutput>;

    public class PaymentMethodCreateHandler : IRequestHandler<PaymentMethodCreateCommand, PaymentMethodOutput>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PaymentMethodCreateHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PaymentMethodOutput> Handle(PaymentMethodCreateCommand request, CancellationToken cancellationToken)
        {
            var paymentMethod = await _paymentMethodRepository.CreateAsync(request.Adapt<PaymentMethod>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return paymentMethod.Adapt<PaymentMethodOutput>();
        }
    }
}
