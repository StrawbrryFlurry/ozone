using Ozone.Common.Identification;

namespace Ozone.Identity.Core.Authorization.Provider;

public sealed record ExternalAuthenticationCallback {
  public required CorrelationId CorrelationId { get; init; }
  public string? Code { get; init; }
  public string? IdToken { get; init; }
  public string? Error { get; init; }
  public string? ErrorDescription { get; init; }
}