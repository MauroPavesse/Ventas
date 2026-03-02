using Ventas.Domain.Common;

namespace Ventas.Application.Entities.TaxConditions.DTOs
{
    public class TaxConditionOutput : BaseModel
    {
        public int Code { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
