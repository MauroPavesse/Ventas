using Mapster;
using MediatR;
using Ventas.Application.Entities.Categories.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

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
            var category = await categoryRepository.CreateAsync(request.Adapt<Category>());
            await unitOfWorkRepository.SaveChangesAsync();
            return category.Adapt<CategoryOutput>();
        }
    }
}
