using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(t => t.Username)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");
        }
    }
}
