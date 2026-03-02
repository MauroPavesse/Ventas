using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class Entity : BaseModel
    {
        public string Name { get; set; } = string.Empty;

        public IEnumerable<StateEntity> StateEntities { get; set; } = new List<StateEntity>();
    }
}
