using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.PaymentMethods.Delete
{
    public record PaymentMethodDeleteCommand(int Id) : IRequest<bool>;

    public class PaymentMethodDeleteHandler : IRequestHandler<PaymentMethodDeleteCommand, bool>
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        
        public PaymentMethodDeleteHandler(IPaymentMethodRepository paymentMethodRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }
        
        public async Task<bool> Handle(PaymentMethodDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingPaymentMethod = await _paymentMethodRepository.GetByIdAsync(request.Id);
            if(existingPaymentMethod == null)
            {
                throw new KeyNotFoundException($"Método de pago con ID {request.Id} no encontrado.");
            }
            existingPaymentMethod.Deleted = 1;
            existingPaymentMethod.Active = 0;
            var result = await _paymentMethodRepository.UpdateAsync(existingPaymentMethod);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
