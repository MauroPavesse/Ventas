using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Vouchers
{
    public class VoucherValidator : AbstractValidator<Voucher>
    {
        public VoucherValidator()
        {
            RuleFor(t => t.Number)
                .NotNull().WithMessage("El número es obligatorio.");
        }
    }
}
