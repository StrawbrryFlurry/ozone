namespace Ozone.Common.Testing.Api.Request;

public sealed class FormEncodedContentBuilder : Dictionary<string, string> {
  public FormUrlEncodedContent Build() {
    return new FormUrlEncodedContent(this);
  }
}