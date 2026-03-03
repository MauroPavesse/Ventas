using Ventas.Domain.Common;

namespace Ventas.Application.Entities.VoucherTypes.DTOs
{
    public class VoucherTypeOutput : BaseModel
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
