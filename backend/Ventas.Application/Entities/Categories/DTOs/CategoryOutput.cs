using Ventas.Domain.Common;

namespace Ventas.Application.Entities.Categories.DTOs
{
    public class CategoryOutput : BaseModel
    {
        public string Name { get; set; } = string.Empty;
    }
}
