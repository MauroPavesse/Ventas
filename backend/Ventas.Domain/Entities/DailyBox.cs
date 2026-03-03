using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class DailyBox : BaseModel
    {
        public int Number { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}
