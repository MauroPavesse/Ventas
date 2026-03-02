using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class PaymentMethod : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal DescountPercentage { get; set; }
        public decimal IncreasePercentage { get; set; }
        public string Color { get; set; } = string.Empty;

        public IEnumerable<VoucherPayment> VoucherPayments { get; set; } = new List<VoucherPayment>();
    }
}
