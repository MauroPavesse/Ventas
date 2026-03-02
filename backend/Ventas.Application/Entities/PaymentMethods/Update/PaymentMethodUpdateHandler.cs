using Mapster;
using MediatR;
using Ventas.Application.Entities.PaymentMethods.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.PaymentMethods.Update
{
    public record PaymentMethodUpdateCommand(int Id, string Name, decimal DescountPercentage, decimal IncreasePercentage, string Color) : IRequest<PaymentMethodOutput>;

    public class PaymentMethodUpdateHandler : IRequestHandler<PaymentMethodUpdateCommand, PaymentMethodOutput>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PaymentMethodUpdateHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PaymentMethodOutput> Handle(PaymentMethodUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingPaymentMethod = await _paymentMethodRepository.GetByIdAsync(request.Id);
            if(existingPaymentMethod == null)
            {
                throw new KeyNotFoundException($"Método de pago con Id {request.Id} no encontrado.");
            }
            existingPaymentMethod.Name = request.Name;
            existingPaymentMethod.DescountPercentage = request.DescountPercentage;
            existingPaymentMethod.IncreasePercentage = request.IncreasePercentage;
            existingPaymentMethod.Color = request.Color;
            var updatedPaymentMethod = await _paymentMethodRepository.UpdateAsync(existingPaymentMethod);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedPaymentMethod.Adapt<PaymentMethodOutput>();
        }
    }
}
