namespace Ozone.Identity.Sdk;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireClaimsAttribute : Attribute {
  public string[] Claims;

  public RequireClaimsAttribute(params string[] claims) {
    Claims = claims;
  }
}