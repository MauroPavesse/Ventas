using Ventas.Application.Entities.Products.DTOs;
using Ventas.Domain.Common;

namespace Ventas.Application.Entities.VoucherDetails.DTOs
{
    public class VoucherDetailOutput : BaseModel
    {
        public int VoucherId { get; set; }
        public int ProductId { get; set; }
        public ProductOutput? Product { get; set; } = null;
        public string ProductName => Product != null ? Product.Name : string.Empty;
        public decimal Quantity { get; set; }
        public decimal PriceUnit { get; set; }
        public decimal AmountNet { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountFinal { get; set; }
    }
}
