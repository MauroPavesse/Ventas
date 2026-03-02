using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Customers.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Customers.Search
{
    public record CustomerSearchCommand(SearchCommand Search) : IRequest<IEnumerable<CustomerOutput>>;
    
    public class CustomerSearchHandler : IRequestHandler<CustomerSearchCommand, IEnumerable<CustomerOutput>>
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerSearchHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerOutput>> Handle(CustomerSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<Customer, bool>> predicate = t => t.Deleted == 0;

            if(search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var taxConditionIdFilter = search.Filters.FirstOrDefault(t => t.Field == "TaxConditionId");
                if(taxConditionIdFilter != null)
                {
                    predicate = predicate.And(t => t.TaxConditionId == Convert.ToInt32(taxConditionIdFilter.Value));
                }
            }

            var customers = await _customerRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return customers.Adapt<IEnumerable<CustomerOutput>>();
        }
    }
}
