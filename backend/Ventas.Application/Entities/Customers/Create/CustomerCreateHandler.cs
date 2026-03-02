using Mapster;
using MediatR;
using Ventas.Application.Entities.Customers.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Customers.Create
{
    public record CustomerCreateCommand(int Document, string Cuit, string FirstName, string LastName, int TaxConditionId) : IRequest<CustomerOutput>;

    public class CustomerCreateHandler : IRequestHandler<CustomerCreateCommand, CustomerOutput>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CustomerCreateHandler(ICustomerRepository customerRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _customerRepository = customerRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<CustomerOutput> Handle(CustomerCreateCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.CreateAsync(request.Adapt<Customer>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return customer.Adapt<CustomerOutput>();
        }
    }
}
