namespace Ventas.Api.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Ventas.Domain.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        // Creamos el objeto base siguiendo el estándar RFC 7807
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Detail = exception.Message
        };

        // Switch de tipos para asignar Status y Títulos correctos
        switch (exception)
        {
            case ValidationException fluentEx:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Error de validación";
                problemDetails.Detail = "Se encontraron uno o más errores de validación.";
                problemDetails.Extensions["errors"] = fluentEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );
                break;

            case BaseException baseEx: // Captura NotFoundException, BusinessException, etc.
                problemDetails.Status = baseEx.StatusCode;
                problemDetails.Title = "Error de Aplicación";
                break;

            case UnauthorizedAccessException:
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "No Autorizado";
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Error de Servidor";
                // En producción podrías querer ocultar el Detail si es un error genérico
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Indica que la excepción fue manejada
    }
}