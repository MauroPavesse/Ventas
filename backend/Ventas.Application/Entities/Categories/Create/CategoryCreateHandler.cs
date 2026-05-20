using Mapster;
using MediatR;
using Ventas.Application.Entities.Categories.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;
using Ventas.Domain.Exceptions;

namespace Ventas.Application.Entities.Categories.Create
{
    public record CategoryCreateCommand(string Name) : IRequest<CategoryOutput>;

    public class CategoryCreateHandler : IRequestHandler<CategoryCreateCommand, CategoryOutput>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;

        public CategoryCreateHandler(ICategoryRepository categoryRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<CategoryOutput> Handle(CategoryCreateCommand request, CancellationToken cancellationToken)
        {
            // Regla de negocio: Validar duplicados
            var exists = await categoryRepository.SearchAsync(t => t.Name.ToLower() == request.Name.ToLower() && t.Active == 1);
            if (exists.Any())
            {
                throw new BusinessException($"Ya existe una categoría con el nombre '{request.Name}'.");
            }

            var category = await categoryRepository.CreateAsync(request.Adapt<Category>());
            await unitOfWorkRepository.SaveChangesAsync();
            return category.Adapt<CategoryOutput>();
        }
    }
}
