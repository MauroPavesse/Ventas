using Ventas.Application.Entities.PaymentMethods.DTOs;
using Ventas.Domain.Common;

namespace Ventas.Application.Entities.VoucherPayments.DTOs
{
    public class VoucherPaymentOutput : BaseModel
    {
        public decimal Amount { get; set; }
        public int VoucherId { get; set; }
        public int PaymentMethodId { get; set; }
        public PaymentMethodOutput? PaymentMethod { get; set; } = null;
    }
}
