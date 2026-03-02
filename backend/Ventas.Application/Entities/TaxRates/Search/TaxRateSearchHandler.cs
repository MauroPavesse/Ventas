using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.TaxRates.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.TaxRates.Search
{
    public record TaxRateSearchCommand(SearchCommand Search) : IRequest<IEnumerable<TaxRateOutput>>;

    public class TaxRateSearchHandler : IRequestHandler<TaxRateSearchCommand, IEnumerable<TaxRateOutput>>
    {
        private readonly ITaxRateRepository _taxRateRepository;

        public TaxRateSearchHandler(ITaxRateRepository taxRateRepository)
        {
            _taxRateRepository = taxRateRepository;
        }

        public async Task<IEnumerable<TaxRateOutput>> Handle(TaxRateSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<TaxRate, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var taxRates = await _taxRateRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return taxRates.Adapt<IEnumerable<TaxRateOutput>>();
        }
    }
}
