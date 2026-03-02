using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class TaxRate : BaseModel
    {
        public string Description { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public string Code { get; set; } = string.Empty;

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
