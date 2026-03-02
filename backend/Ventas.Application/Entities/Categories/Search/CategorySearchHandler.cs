using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Categories.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Categories.Search
{
    public record CategorySearchCommand(SearchCommand Search) : IRequest<IEnumerable<CategoryOutput>>;

    public class CategorySearchHandler : IRequestHandler<CategorySearchCommand, IEnumerable<CategoryOutput>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategorySearchHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryOutput>> Handle(CategorySearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<Category, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var categories = await _categoryRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return categories.Adapt<IEnumerable<CategoryOutput>>();
        }
    }
}
