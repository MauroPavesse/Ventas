using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Configuration : BaseModel
    {
        public string Variable { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StringValue { get; set; } = string.Empty;
        public decimal NumericValue { get; set; }
        public bool BoolValue { get; set; }
    }
}
