using Ventas.Domain.Common;

namespace Ventas.Application.Entities.PaymentMethods.DTOs
{
    public class PaymentMethodOutput : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal DescountPercentage { get; set; }
        public decimal IncreasePercentage { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}
