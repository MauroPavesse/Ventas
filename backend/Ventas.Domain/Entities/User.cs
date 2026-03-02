using Ventas.Domain.Common;

namespace Ventas.Domain.Entities
{
    public class User : BaseModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public Role? Role { get; set; } = null;
        public int? PointOfSaleId { get; set; }
        public PointOfSale? PointOfSale { get; set; } = null;
    }
}
