using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Domain.Common;

namespace Ventas.Application.Entities.DailyBoxes.DTOs
{
    public class DailyBoxOutput : BaseModel
    {
        public int Number { get; set; }
        public decimal Amount { get; set; }
        public List<VoucherOutput> Vouchers { get; set; } = new List<VoucherOutput>();
        public int QuantityVouchers => Vouchers.Count;
    }
}
