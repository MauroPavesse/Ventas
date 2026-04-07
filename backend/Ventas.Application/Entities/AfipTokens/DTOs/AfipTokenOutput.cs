using Ventas.Domain.Common;

namespace Ventas.Application.Entities.AfipTokens.DTOs
{
    public class AfipTokenOutput : BaseModel
    {
        public string Token { get; set; } = string.Empty;
        public string Sign { get; set; } = string.Empty;
        public DateTimeOffset Expiration { get; set; }
    }
}
