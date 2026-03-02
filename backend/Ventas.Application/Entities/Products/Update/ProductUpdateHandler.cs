using Mapster;
using MediatR;
using Ventas.Application.Entities.Products.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Products.Update
{
    public record ProductUpdateCommand(int Id, int? Code, string Name, string Description, string ImagePath, decimal Price, string CodeBar, int CategoryId, int TaxRateId) : IRequest<ProductOutput>;

    public class ProductUpdateHandler : IRequestHandler<ProductUpdateCommand, ProductOutput>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public ProductUpdateHandler(IProductRepository productRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _productRepository = productRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<ProductOutput> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Producto con Id {request.Id} no encontrado.");
            }
            existingProduct.Code = request.Code;
            existingProduct.Name = request.Name;
            existingProduct.Description = request.Description;
            existingProduct.ImagePath = request.ImagePath;
            existingProduct.Price = request.Price;
            existingProduct.CodeBar = request.CodeBar;
            existingProduct.CategoryId = request.CategoryId;
            existingProduct.TaxRateId = request.TaxRateId;
            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedProduct.Adapt<ProductOutput>();
        }
    }
}
