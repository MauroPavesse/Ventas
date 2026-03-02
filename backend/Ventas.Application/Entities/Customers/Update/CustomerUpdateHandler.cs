using Mapster;
using MediatR;
using Ventas.Application.Entities.Customers.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Customers.Update
{
    public record CustomerUpdateCommand(int Id, int Document, string Cuit, string FirstName, string LastName, int TaxConditionId) : IRequest<CustomerOutput>;

    public class CustomerUpdateHandler : IRequestHandler<CustomerUpdateCommand, CustomerOutput>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CustomerUpdateHandler(ICustomerRepository customerRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _customerRepository = customerRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<CustomerOutput> Handle(CustomerUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.GetByIdAsync(request.Id);
            if(existingCustomer == null)
            {
                throw new KeyNotFoundException($"Cliente con Id {request.Id} no encontrado.");
            }
            existingCustomer.Document = request.Document;
            existingCustomer.Cuit = request.Cuit;
            existingCustomer.FirstName = request.FirstName;
            existingCustomer.LastName = request.LastName;
            existingCustomer.TaxConditionId = request.TaxConditionId;
            var updatedCustomer = await _customerRepository.UpdateAsync(existingCustomer);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedCustomer.Adapt<CustomerOutput>();
        }
    }
}
