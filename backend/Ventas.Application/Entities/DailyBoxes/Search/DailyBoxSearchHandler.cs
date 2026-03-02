using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.DailyBoxes.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.DailyBoxes.Search
{
    public record DailyBoxSearchCommand(SearchCommand Search) : IRequest<IEnumerable<DailyBoxOutput>>;

    public class DailyBoxSearchHandler : IRequestHandler<DailyBoxSearchCommand, IEnumerable<DailyBoxOutput>>
    {
        private readonly IDailyBoxRepository _dailyBoxRepository;

        public DailyBoxSearchHandler(IDailyBoxRepository dailyBoxRepository)
        {
            _dailyBoxRepository = dailyBoxRepository;
        }

        public async Task<IEnumerable<DailyBoxOutput>> Handle(DailyBoxSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<DailyBox, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var dateFilter = search.Filters.FirstOrDefault(t => t.Field == "Date");
                if (dateFilter != null)
                {
                    predicate = predicate.And(t => t.Date.Date == Convert.ToDateTime(dateFilter.Value));
                }
            }

            var dailyBoxes = await _dailyBoxRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return dailyBoxes.Adapt<IEnumerable<DailyBoxOutput>>();
        }
    }
}
