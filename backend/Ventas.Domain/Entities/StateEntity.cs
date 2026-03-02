using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class StateEntity : BaseModel
    {
        public string State { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public Entity? Entity { get; set; } = null;
    }
}
