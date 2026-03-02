using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class VoucherPayment : BaseModel
    {
        public decimal Amount { get; set; }
        public int VoucherId { get; set; }
        public Voucher? Voucher { get; set; } = null;
        public int PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; } = null;
    }
}
