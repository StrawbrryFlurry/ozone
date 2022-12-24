using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Ozone.Common.Domain.Data;
using Ozone.Common.Functional;
using static System.Text.RegularExpressions.RegexOptions;

namespace Ozone.Common.Domain.Identity;

public sealed class IdentityDescriptor : ValueObject {
  /// <summary>
  /// Matches an identity descriptor with the following pattern:
  /// [Service-Scope-Namespace]:[Fully.Qualified.Name]
  /// Ozone-Automation:Infrastructure.Script.Invoke
  /// - Namespace group is optional
  /// - Namespace cannot start or end with a dash
  /// - If namespace is present, it must be followed by a colon
  /// - Action descriptor must be present
  /// - Action descriptor cannot start or end with a dot
  /// </summary>
  public static readonly Regex IdentityDescriptorRegex = new(
    @"^(?:(?<NameSpace>(?:[a-z]+(?:-[a-z]+)?)+):)?(?<Name>(?:[a-z0-9]+(?:\.|-[a-z0-9]+)?)+)$",
    Compiled | IgnoreCase | Singleline
  );

  public string Name { get; private set; }
  public string? Namespace { get; private set; }

  public string Descriptor { get; private set; }

  [PersistenceConstructor]
  private IdentityDescriptor() { }

  private IdentityDescriptor(string name, string? ns, string descriptor) {
    Name = name;
    Namespace = ns;
    Descriptor = descriptor;
  }

  public static Result<IdentityDescriptor> Create(string name) {
    var descriptor = FormatIdentityDescriptorSegment(name);

    if (!IsValid(descriptor)) {
      return IdentityDescriptorErrors.InvalidIdentityDescriptorFormat;
    }

    return new IdentityDescriptor(name, null, descriptor);
  }

  public static Result<IdentityDescriptor> Create(string name, string? ns) {
    if (ns is null) {
      return Create(name);
    }

    var descriptor = $"{FormatIdentityDescriptorSegment(name)}:{FormatIdentityDescriptorSegment(ns)}";

    if (!IdentityDescriptorRegex.IsMatch(descriptor)) {
      return IdentityDescriptorErrors.InvalidIdentityDescriptorFormat;
    }

    return new IdentityDescriptor(name, ns, descriptor);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string FormatIdentityDescriptorSegment(string segment) {
    return segment.Replace(" ", "-");
  }

  public static Result<IdentityDescriptor> CreateFrom(string descriptor) {
    var match = IdentityDescriptorRegex.Match(descriptor);

    if (!match.Success) {
      return IdentityDescriptorErrors.InvalidIdentityDescriptorFormat;
    }

    string? ns = null;
    if (match.Groups["NameSpace"].Success) {
      ns = match.Groups["NameSpace"].Value;
    }

    var name = match.Groups["Name"].Value;

    return new IdentityDescriptor(name, ns, descriptor);
  }

  public static bool IsValid(string descriptor) {
    return IdentityDescriptorRegex.IsMatch(descriptor);
  }

  public bool HasNamespace() {
    return !string.IsNullOrWhiteSpace(Namespace);
  }

  protected override IEnumerable<object?> GetEqualityComponents() {
    yield return Namespace;
    yield return Name;
  }

  public static implicit operator string(IdentityDescriptor descriptor) {
    return descriptor.Descriptor;
  }

  public override string ToString() {
    return Descriptor;
  }
}