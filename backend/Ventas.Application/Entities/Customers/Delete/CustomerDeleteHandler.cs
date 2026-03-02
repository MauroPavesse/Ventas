using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Customers.Delete
{
    public record CustomerDeleteCommand(int Id) : IRequest<bool>;

    public class CustomerDeleteHandler : IRequestHandler<CustomerDeleteCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CustomerDeleteHandler(ICustomerRepository customerRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _customerRepository = customerRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(CustomerDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(request.Id);
            if(existingCustomer == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {request.Id} no encontrado.");
            }
            existingCustomer.Deleted = 1;
            existingCustomer.Active = 0;
            var result = await _customerRepository.UpdateAsync(existingCustomer);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
