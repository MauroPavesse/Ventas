using FluentValidation;
using Ventas.Application.Entities.Categories.Create;

namespace Ventas.Application.Entities.Categories
{
    public class CategoryValidator : AbstractValidator<CategoryCreateCommand>
    {
        public CategoryValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar los 50 caracteres.");
        }
    }
}
