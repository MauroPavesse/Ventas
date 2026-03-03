using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherDetails
{
    public class VoucherDetailValidator : AbstractValidator<VoucherDetail>
    {
        public VoucherDetailValidator()
        {
            RuleFor(t => t.Quantity)
                .NotNull().WithMessage("La cantidad es requerida.");
        }
    }
}
