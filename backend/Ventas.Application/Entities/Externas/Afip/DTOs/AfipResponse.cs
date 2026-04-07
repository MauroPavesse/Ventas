namespace Ventas.Application.Entities.Externas.Afip.DTOs
{
    public class AfipResponse
    {
        public List<AfipErrorOutput> Errors { get; set; } = [];
        public bool Success { get; set; }
        public string? Cae { get; set; }
        public DateTime? CaeExpiration { get; set; }

        private AfipResponse() { }

        public static AfipResponse Ok(string cae, DateTime caeExpiration)
        {
            return new AfipResponse
            {
                Success = true,
                Cae = cae,
                CaeExpiration = caeExpiration
            };
        }

        public static AfipResponse Fail(List<AfipErrorOutput> errors)
        {
            return new AfipResponse
            {
                Success = false,
                Errors = errors
            };
        }
    }
}
