using System.ComponentModel.DataAnnotations;

namespace Ventas.Domain.Common
{
    public abstract class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public int Deleted { get; set; } = 0;
        public int Active { get; set; } = 1;
    }
}
