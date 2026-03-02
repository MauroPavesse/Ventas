using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Category : BaseModel
    {
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
