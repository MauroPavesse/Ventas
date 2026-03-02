using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Role : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}
