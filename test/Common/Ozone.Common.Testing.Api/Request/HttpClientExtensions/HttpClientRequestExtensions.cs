namespace Ozone.Common.Testing.Api.Request.HttpClientExtensions;

public static class HttpClientRequestExtensions {
  public static Task<HttpResponseMessage> GetAsync(
    this HttpClient client,
    string uri,
    HttpContent content
  ) {
    var request = new HttpRequestMessage {
      Content = content,
      Method = HttpMethod.Get,
      RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute)
    };

    return client.SendAsync(request);
  }
}