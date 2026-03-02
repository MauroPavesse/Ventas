using Ventas.Domain.Common;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users.DTOs
{
    public class UserOutput : BaseModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public int? PointOfSaleId { get; set; }
    }
}
