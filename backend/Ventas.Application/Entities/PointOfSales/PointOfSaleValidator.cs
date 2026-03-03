using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSales
{
    public class PointOfSaleValidator : AbstractValidator<PointOfSale>
    {
        public PointOfSaleValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar los 50 caracteres.");
        }
    }
}
