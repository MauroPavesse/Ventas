using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Product : BaseModel
    {
        public int? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CodeBar { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null;
        public int TaxRateId { get; set; }
        public TaxRate? TaxRate { get; set; } = null;

        public IEnumerable<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
    }
}
