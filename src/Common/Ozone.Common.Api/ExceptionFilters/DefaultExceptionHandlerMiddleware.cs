using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ozone.Common.Api.ExceptionFilters;

public sealed class DefaultExceptionHandlerMiddleware : IMiddleware {
  private readonly ILogger _logger;

  public DefaultExceptionHandlerMiddleware(ILogger logger) {
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
    try {
      await next(context);
    }
    catch (Exception ex) {
      _logger.Log(LogLevel.Error,
        "Encountered unhandled exception. Type: {type}. Message: {mesasge}. Stack: {stack}",
        ex.Message, ex.GetType(), ex.StackTrace
      );
      WriteResponseMessage(context.Response, ex);
    }
  }

  private void WriteResponseMessage(HttpResponse response, Exception ex) {
    var problemDetails = new ProblemDetails {
      Status = StatusCodes.Status500InternalServerError,
      Type = "Error.Internal",
      Detail = "Invalid server error"
    };
  }
}