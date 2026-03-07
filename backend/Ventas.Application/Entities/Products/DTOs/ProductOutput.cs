using Ventas.Application.Entities.Categories.DTOs;
using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Products.DTOs
{
    public class ProductOutput : BaseModel
    {
        public string? Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CodeBar { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public CategoryOutput? Category { get; set; } = null;
        public int TaxRateId { get; set; }
    }
}
