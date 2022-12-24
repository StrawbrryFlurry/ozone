using Microsoft.AspNetCore.Http;

namespace Ozone.Identity.Core.Authorization.Provider;

public interface IAuthorizationTicket {
  /// <summary>
  ///   Performs the authentication for the current request.
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  public Task InvokeAsync(HttpContext context);
}