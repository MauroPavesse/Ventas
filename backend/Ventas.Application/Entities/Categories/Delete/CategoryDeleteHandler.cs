using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Categories.Delete
{
    public record CategoryDeleteCommand(int Id) : IRequest<bool>;

    public class CategoryDeleteHandler : IRequestHandler<CategoryDeleteCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CategoryDeleteHandler(ICategoryRepository categoryRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _categoryRepository = categoryRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(request.Id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Categoría con ID {request.Id} no encontrado.");
            }
            existingCategory.Deleted = 1;
            existingCategory.Active = 0;
            var result = await _categoryRepository.UpdateAsync(existingCategory);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
