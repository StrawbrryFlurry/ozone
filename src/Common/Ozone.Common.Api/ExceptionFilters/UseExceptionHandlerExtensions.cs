using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Ozone.Common.Api.ExceptionFilters;

public static class UseExceptionFilterExtensions {
  public static void UseDefaultExceptionHandler(this IApplicationBuilder builder) {
    builder.UseMiddleware<DefaultExceptionHandlerMiddleware>();
  }
}