using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Products.Delete
{
    public record ProductDeleteCommand(int Id) : IRequest<bool>;

    public class ProductDeleteHandler : IRequestHandler<ProductDeleteCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public ProductDeleteHandler(IProductRepository productRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _productRepository = productRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetByIdAsync(request.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Producto con ID {request.Id} no encontrado.");
            }
            existingProduct.Deleted = 1;
            existingProduct.Active = 0;
            var result = await _productRepository.UpdateAsync(existingProduct);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
