using Carvia.Core.Exceptions;
using Carvia.Core.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace Carvia.Infrastructure.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
  private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
  {
    var statusCode = exception switch
    {
      NotFoundException => StatusCodes.Status404NotFound,
      AuthorizationException => StatusCodes.Status401Unauthorized,
      ForbiddenException => StatusCodes.Status403Forbidden,
      BusinessException => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    var isJsonRequest = httpContext.Request.Headers.Accept.ToString().Contains("application/json") ||
                        httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

    if (isJsonRequest)
    {
      var response = new ReturnModel<NoData>
      {
        Success = false,
        Message = statusCode == 500 ? "Sistem kaynaklı bir hata oluştu." : exception.Message,
        StatusCode = statusCode
      };

      httpContext.Response.ContentType = "application/json";
      httpContext.Response.StatusCode = statusCode;

      await httpContext.Response.WriteAsync(
        JsonSerializer.Serialize(response, _jsonOptions),
        cancellationToken);

      return true;
    }

    return false;
  }
}
