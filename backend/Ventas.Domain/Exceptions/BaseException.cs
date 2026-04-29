using Microsoft.AspNetCore.Http;

namespace Ventas.Domain.Exceptions
{
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; }
        protected BaseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound) { }
    }

    public class BusinessException : BaseException
    {
        // Para errores de lógica de negocio (ej. "Stock insuficiente")
        public BusinessException(string message) : base(message, StatusCodes.Status400BadRequest) { }
    }
}
