using Ventas.Domain.Common;
using Ventas.Domain.Enums;

namespace Ventas.Domain.Entities
{
    public class Product : BaseModel
    {
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string CodeBar { get; set; } = string.Empty;
        public int? CategoryId { get; set; } = null;
        public Category? Category { get; set; } = null;
        public int TaxRateId { get; set; }
        public TaxRate? TaxRate { get; set; } = null;
        public UnitTypeEnum UnitOfMeasurement { get; set; } = UnitTypeEnum.UNIDAD;

        public IEnumerable<VoucherDetail> VoucherDetails { get; set; } = new List<VoucherDetail>();
    }
}
