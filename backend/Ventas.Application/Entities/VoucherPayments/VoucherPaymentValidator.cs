using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherPayments
{
    public class VoucherPaymentValidator : AbstractValidator<VoucherPayment>
    {
        public VoucherPaymentValidator()
        {
        }
    }
}
