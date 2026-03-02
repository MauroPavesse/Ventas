using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Vouchers.DTOs
{
    public class VoucherOutput : BaseModel
    {
        public int Number { get; set; }
        public decimal AmountNet { get; set; }
        public decimal AmountVAT { get; set; }
        public string CAE { get; set; } = string.Empty;
        public DateTime CAEExpiration { get; set; }
        public int UserId { get; set; }
        public int? CustomerId { get; set; }
        public int VoucherTypeId { get; set; }
        public int? DailyBoxId { get; set; }
        public DateTime DateCreation { get; set; }
        public int StateEntityId { get; set; }
    }
}
