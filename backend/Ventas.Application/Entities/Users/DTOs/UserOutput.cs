using Ventas.Application.Entities.PointOfSales.DTOs;
using Ventas.Application.Entities.Roles.DTOs;
using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Users.DTOs
{
    public class UserOutput : BaseModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public RoleOutput? Role { get; set; } = null;
        public string RoleName => Role != null ? Role.Name : "";
        public int? PointOfSaleId { get; set; }
        public PointOfSaleOutput? PointOfSale { get; set; } = null;
        public string PointOfSaleName => PointOfSale != null ? PointOfSale.Name : "";
    }
}
