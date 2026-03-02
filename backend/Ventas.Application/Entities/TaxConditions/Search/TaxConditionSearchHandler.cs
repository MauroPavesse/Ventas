using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.TaxConditions.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.TaxConditions.Search
{
    public record TaxConditionSearchCommand(SearchCommand Search) : IRequest<IEnumerable<TaxConditionOutput>>;

    public class TaxConditionSearchHandler : IRequestHandler<TaxConditionSearchCommand, IEnumerable<TaxConditionOutput>>
    {
        private readonly ITaxConditionRepository _taxConditionRepository;

        public TaxConditionSearchHandler(ITaxConditionRepository taxConditionRepository)
        {
            _taxConditionRepository = taxConditionRepository;
        }

        public async Task<IEnumerable<TaxConditionOutput>> Handle(TaxConditionSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<TaxCondition, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var taxConditions = await _taxConditionRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return taxConditions.Adapt<IEnumerable<TaxConditionOutput>>();
        }
    }
}
