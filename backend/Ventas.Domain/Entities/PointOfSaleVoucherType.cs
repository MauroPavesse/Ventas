using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class PointOfSaleVoucherType : BaseModel
    {
        public int PointOfSaleId { get; set; }
        public PointOfSale PointOfSale { get; set; } = null!;
        public int VoucherTypeId { get; set; }
        public VoucherType VoucherType { get; set; } = null!;
        public int Numeration { get; set; }
    }
}
