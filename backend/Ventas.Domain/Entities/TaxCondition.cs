using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class TaxCondition : BaseModel
    {
        public int Code { get; set; }
        public string Description { get; set; } = string.Empty;

        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
    }
}
