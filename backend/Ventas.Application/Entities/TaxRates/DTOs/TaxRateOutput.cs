using Ventas.Domain.Common;

namespace Ventas.Application.Entities.TaxRates.DTOs
{
    public class TaxRateOutput : BaseModel
    {
        public string Description { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
