using Ventas.Domain.Common;

namespace Ventas.Application.Entities.DailyBoxes.DTOs
{
    public class DailyBoxOutput : BaseModel
    {
        public int Number { get; set; }
        public decimal Amount { get; set; }
    }
}
