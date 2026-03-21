using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Voucher : BaseModel
    {
        public int Number { get; set; }
        public decimal AmountNet { get; set; }
        public decimal AmountVAT { get; set; }
        public string CAE { get; set; } = string.Empty;
        public DateTime? CAEExpiration { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } = null;
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; } = null;
        public int VoucherTypeId { get; set; }
        public VoucherType? VoucherType { get; set; } = null;
        public int? DailyBoxId { get; set; }
        public DailyBox? DailyBox { get; set; } = null;
        public DateTime DateCreation { get; set; }
        public int StateEntityId { get; set; }
        public StateEntity? StateEntity { get; set; } = null;

        public IEnumerable<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
        public IEnumerable<VoucherPayment> VoucherPayments { get; set; } = new List<VoucherPayment>();
    }
}
