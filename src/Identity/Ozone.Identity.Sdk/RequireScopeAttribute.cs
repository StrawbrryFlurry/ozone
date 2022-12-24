namespace Ozone.Identity.Sdk;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireScopeAttribute : Attribute {
  public string Scope;

  public RequireScopeAttribute(string scope) {
    Scope = scope;
  }
}