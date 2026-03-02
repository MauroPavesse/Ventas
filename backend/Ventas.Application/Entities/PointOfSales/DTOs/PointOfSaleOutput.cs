using Ventas.Domain.Common;

namespace Ventas.Application.Entities.PointOfSales.DTOs
{
    public class PointOfSaleOutput : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Provincie { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}
