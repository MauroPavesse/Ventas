using Mapster;
using MediatR;
using Ventas.Application.Entities.Products.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Products.Create
{
    public record ProductCreateCommand(string? Code, string Name, string Description, string ImagePath, decimal Price, string CodeBar, int CategoryId, int TaxRateId) : IRequest<ProductOutput>;

    public class ProductCreateHandler : IRequestHandler<ProductCreateCommand, ProductOutput>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public ProductCreateHandler(IProductRepository productRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _productRepository = productRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<ProductOutput> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.CreateAsync(request.Adapt<Product>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return product.Adapt<ProductOutput>();
        }
    }
}
