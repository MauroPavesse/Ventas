using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Customers.DTOs
{
    public class CustomerOutput : BaseModel
    {
        public int Document { get; set; }
        public string Cuit { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int TaxConditionId { get; set; }
    }
}
