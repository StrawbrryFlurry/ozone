using System.Collections;
using Ozone.Common.Domain.Data;
using Ozone.Common.Domain.Identity;
using Ozone.Common.Extensions;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Core.Security.Authorization;

/// <summary>
/// Represents a collection of <see cref="ServiceAction"/>s outside
/// the domain boundary that is was specified by an authorization
/// request or is present in a security token.
/// </summary>
public sealed class AuthorizationKeyChain : ValueObject, IReadOnlyCollection<IdentityDescriptor> {
  private IReadOnlyList<IdentityDescriptor> _actions;

  public int Count => _actions.Count;
  public string? Namespace => _actions[0].Namespace;

  private AuthorizationKeyChain(IReadOnlyList<IdentityDescriptor> actions) {
    _actions = actions;
  }

  public static Result<AuthorizationKeyChain> CreateFrom(string? actionsString) {
    if (string.IsNullOrWhiteSpace(actionsString)) {
      return new AuthorizationKeyChain(Enumerable.Empty<IdentityDescriptor>().ToList());
    }

    var actions = actionsString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var invalidActions = actions.Where(x => !IdentityDescriptor.IsValid(x));

    if (invalidActions.Any()) {
      return ApplicationErrors.Authorization.InvalidKeyChainActionFormat(invalidActions.JoinBy(", "));
    }

    var authorizationActions = actions.Select(IdentityDescriptor.CreateFrom).GetValues().ToList();

    if (ContainsMoreThanOneNamespace(authorizationActions)) {
      return ApplicationErrors.Authorization.MultipleServiceScopesInKeyChain;
    }

    return new AuthorizationKeyChain(authorizationActions);
  }

  private static bool ContainsMoreThanOneNamespace(IEnumerable<IdentityDescriptor> actions) {
    string? ns = null;

    foreach (var action in actions) {
      var actionNamespace = action.Namespace;

      if (actionNamespace is null) {
        actionNamespace = "empty_namespace";
      }

      if (ns is null) {
        ns = actionNamespace;
        continue;
      }

      if (ns != actionNamespace) {
        return true;
      }
    }

    return false;
  }

  protected override IEnumerable<object?> GetEqualityComponents() {
    yield return _actions.Order();
  }

  public IEnumerator<IdentityDescriptor> GetEnumerator() {
    return _actions.AsEnumerable().GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }
}