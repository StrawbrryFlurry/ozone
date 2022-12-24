using Microsoft.AspNetCore.Http;
using Ozone.Common.Functional;

namespace Ozone.Identity.Core.Authorization.Provider.OAuth;

public sealed class RedirectAuthorizationTicket : IAuthorizationTicket {
  public readonly string RedirectUri;

  public RedirectAuthorizationTicket(string redirectUri) {
    RedirectUri = redirectUri;
  }

  public RedirectAuthorizationTicket(Uri redirectUri) {
    RedirectUri = redirectUri.ToString();
  }

  public static RedirectAuthorizationTicket FromError(Uri redirectUri, string state, Error error) {
    var uriBuilder = new UriBuilder(redirectUri);
    var query = new QueryString();

    query = query.Add("state", state);
    query = query.Add("error", error.Code);
    query = query.Add("error_description", error.Message);

    uriBuilder.Query = query.ToString();

    return new RedirectAuthorizationTicket(uriBuilder.Uri);
  }

  public Task InvokeAsync(HttpContext context) {
    context.Response.Redirect(RedirectUri);
    return Task.CompletedTask;
  }
}