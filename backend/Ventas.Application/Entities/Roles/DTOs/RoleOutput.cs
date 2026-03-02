using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Roles.DTOs
{
    public class RoleOutput : BaseModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
