using Ventas.Application.Entities.Users.DTOs;
using Ventas.Application.Entities.VoucherDetails.DTOs;
using Ventas.Application.Entities.VoucherPayments.DTOs;
using Ventas.Application.Entities.VoucherTypes.DTOs;
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
        public UserOutput? User { get; set; } = null;
        public int? CustomerId { get; set; }
        public int VoucherTypeId { get; set; }
        public VoucherTypeOutput? VoucherType { get; set; } = null;
        public int? DailyBoxId { get; set; }
        public DateTime DateCreation { get; set; }
        public int StateEntityId { get; set; }

        public List<VoucherDetailOutput> VoucherDetails { get; set; } = new List<VoucherDetailOutput>();
        public List<VoucherPaymentOutput> VoucherPayments { get; set; } = new List<VoucherPaymentOutput>();
        public string Description => VoucherType != null && User != null && User.PointOfSale != null ? VoucherType.Description + " " + User.PointOfSale.Number.ToString().PadLeft(5, '0') + "-" + Number.ToString().PadLeft(7, '0') : "";
        public decimal AmountTotal => AmountNet + AmountVAT;
    }
}
