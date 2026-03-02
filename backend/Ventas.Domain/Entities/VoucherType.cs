using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class VoucherType : BaseModel
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public IEnumerable<PointOfSaleVoucherType> PointOfSaleVoucherTypes { get; set; } = new List<PointOfSaleVoucherType>();
    }
}
