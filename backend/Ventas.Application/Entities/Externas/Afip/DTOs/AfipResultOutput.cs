namespace Ventas.Application.Entities.Externas.Afip.DTOs
{
    public class AfipResultOutput<T>
    {
        public List<AfipErrorOutput> Errors { get; set; } = [];
        public bool Success => Errors.Count == 0;
        public T? Data { get; set; }
    }
}
