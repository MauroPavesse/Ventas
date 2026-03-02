using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Roles
{
    public class RuleValidator : AbstractValidator<Role>
    {
        public RuleValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar los 50 caracteres.");
        }
    }
}
