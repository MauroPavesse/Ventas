using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class PointOfSale : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Provincie { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public IEnumerable<User> Users { get; set; } = new List<User>();
        public IEnumerable<PointOfSaleVoucherType> PointOfSaleVoucherTypes { get; set; } = new List<PointOfSaleVoucherType>();
        public IEnumerable<AfipToken> AfipTokens { get; set; } = new List<AfipToken>();
    }
}
