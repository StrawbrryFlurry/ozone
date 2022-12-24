using System.Collections;
using Ozone.Common.Domain.Data;
using Ozone.Common.Domain.Identity;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;

namespace Ozone.Identity.Core.Security.Authorization;

/// <summary>
/// Represents a collection of authorization scopes
/// outside of the domain boundary. Note that it
/// is not verified that the scopes are valid or if they
/// even exist in the system.
/// </summary>
public sealed class AuthorizationScopes : ValueObject, IReadOnlyCollection<IdentityDescriptor> {
  private readonly List<IdentityDescriptor> _scopes;

  protected override IEnumerable<object?> GetEqualityComponents() {
    return _scopes.Order();
  }

  private AuthorizationScopes(List<IdentityDescriptor> scopes) {
    _scopes = scopes;
  }

  /// <summary>
  /// Parses a scope string (space separated list) into a collection of scopes.
  /// </summary>
  /// <param name="scopes"></param>
  /// <returns></returns>
  public static Result<AuthorizationScopes> CreateFrom(string scopes) {
    if (string.IsNullOrWhiteSpace(scopes)) {
      return new AuthorizationScopes(new List<IdentityDescriptor>());
    }

    var parsedScopesResult = scopes.Split(' ').Select(IdentityDescriptor.CreateFrom).ToList();

    if (parsedScopesResult.HasError(out var error)) {
      return error;
    }

    return new AuthorizationScopes(parsedScopesResult.GetValues().ToList());
  }

  public IEnumerator<IdentityDescriptor> GetEnumerator() {
    return _scopes.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  public int Count => _scopes.Count;
}