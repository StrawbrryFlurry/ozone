using Ozone.Common.Identification;

namespace Ozone.Common.Domain.Identity;

public static class IdentityDescriptorExtensions {
  public static List<IdentityDescriptor> DefineNamespaceIfNotSet(
    this IEnumerable<IdentityDescriptor> identityDescriptor, string ns) {
    var result = new List<IdentityDescriptor>();
    foreach (var descriptor in identityDescriptor) {
      if (descriptor.HasNamespace()) {
        result.Add(descriptor);
        continue;
      }

      result.Add(IdentityDescriptor.Create(descriptor.Name, ns).Value);
    }

    return result;
  }
}