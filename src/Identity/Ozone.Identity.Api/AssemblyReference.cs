using System.Reflection;

namespace Ozone.Identity.Api;

public sealed class AssemblyReference {
  public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}