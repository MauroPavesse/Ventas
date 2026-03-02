using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class VoucherDetail : BaseModel
    {
        public int VoucherId { get; set; }
        public Voucher? Voucher { get; set; } = null;
        public int ProductId { get; set; }
        public Product? Product { get; set; } = null;
        public decimal Quantity { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal AmountNet { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountFinal { get; set; }
    }
}
