using Ventas.Domain.Common;

namespace Ventas.Application.Entities.PointOfSaleVoucherTypes.DTOs
{
    public class PointOfSaleVoucherTypeOutput : BaseModel
    {
        public int PointOfSaleId { get; set; }
        public int VoucherTypeId { get; set; }
        public int Numeration { get; set; }
    }
}
