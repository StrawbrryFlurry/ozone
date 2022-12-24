namespace Ozone.Common.Testing.Api.Extensions;

public static class HttpResponseExtensions {
  public static string GetBodyAsString(this HttpResponseMessage response) {
    return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
  }
}