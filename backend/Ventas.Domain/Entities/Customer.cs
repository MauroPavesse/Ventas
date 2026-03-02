using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Customer : BaseModel
    {
        public int Document { get; set; }
        public string Cuit { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int TaxConditionId { get; set; }
        public TaxCondition? TaxCondition { get; set; } = null;

        public IEnumerable<Voucher> Vouchers { get; set; } = new List<Voucher>();
    }
}
