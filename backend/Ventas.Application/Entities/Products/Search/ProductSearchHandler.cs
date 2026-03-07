using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Products.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Products.Search
{
    public record ProductSearchCommand(SearchCommand Search) : IRequest<IEnumerable<ProductOutput>>;

    public class ProductSearchHandler : IRequestHandler<ProductSearchCommand, IEnumerable<ProductOutput>>
    {
        private readonly IProductRepository _productRepository;

        public ProductSearchHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductOutput>> Handle(ProductSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<Product, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var codeFilter = search.Filters.FirstOrDefault(t => t.Field == "Code");
                if (codeFilter != null)
                {
                    predicate = predicate.And(t => t.Code == codeFilter.Value);
                }
            }

            var products = await _productRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return products.Adapt<IEnumerable<ProductOutput>>();
        }
    }
}
