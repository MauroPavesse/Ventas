using Mapster;
using MediatR;
using Ventas.Application.Entities.Categories.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Categories.Update
{
    public record CategoryUpdateCommand(int Id, string Name) : IRequest<CategoryOutput>;

    public class CategoryUpdateHandler : IRequestHandler<CategoryUpdateCommand, CategoryOutput>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public CategoryUpdateHandler(ICategoryRepository categoryRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<CategoryOutput> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await categoryRepository.GetByIdAsync(request.Id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Categoría con Id {request.Id} no encontrado.");
            }
            existingCategory.Name = request.Name;
            var updatedCategory = await categoryRepository.UpdateAsync(existingCategory);
            await unitOfWorkRepository.SaveChangesAsync();
            return updatedCategory.Adapt<CategoryOutput>();
        }
    }
}
