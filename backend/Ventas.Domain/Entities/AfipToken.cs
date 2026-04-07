using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class AfipToken : BaseModel
    {
        public string Token { get; set; } = string.Empty;
        public string Sign { get; set; } = string.Empty;
        public DateTimeOffset Expiration { get; set; }
        public int PointOfSaleId { get; set; }
        public PointOfSale? PointOfSale { get; set; } = null;
    }
}
