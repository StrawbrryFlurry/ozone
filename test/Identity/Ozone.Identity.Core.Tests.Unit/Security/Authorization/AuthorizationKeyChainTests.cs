using Ozone.Common.Domain.Identity;
using Ozone.Identity.Core.Errors;
using Ozone.Identity.Core.Security.Authorization;
using Ozone.Testing.Common.FluentExtensions;

namespace Ozone.Identity.Core.Tests.Unit.Security.Authorization;

public sealed class AuthorizationKeyChainTests {
  [Fact]
  public void CreateFrom_ReturnsError_WhenActionHasInvalidFormat() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("foo::bar");

    keyChainResult.ShouldFailWith(ApplicationErrors.Authorization.InvalidKeyChainActionFormat("foo::bar"));
  }

  [Fact]
  public void CreateFrom_ReturnsError_WhenKeyChainContainsMultipleScopes() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("foo:bar baz:qux");

    keyChainResult.ShouldFailWith(ApplicationErrors.Authorization.MultipleServiceScopesInKeyChain);
  }

  [Fact]
  public void CreateFrom_ReturnsEmptyKeyChain_WhenInputStringIsNullOrEmpty() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("");

    keyChainResult.Value.Should().BeEmpty();
  }

  [Fact]
  public void CreateFrom_ReturnsParsedKeyChain_WhenInputStringContainsValidActions() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("foo bar.baz qux");

    keyChainResult.Value.Select(x => x.Descriptor).Should().BeEquivalentTo("foo", "bar.baz", "qux");
  }

  [Fact]
  public void CreateFrom_ReturnsParsedKeyChain_WhenInputStringContainsValidActionsWithNamespace() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("scope:foo scope:bar.baz scope:qux");

    keyChainResult.Value.Select(x => x.Name).Should().BeEquivalentTo("foo", "bar.baz", "qux");
    keyChainResult.Value.Select(x => x.Namespace).Should().BeEquivalentTo("scope", "scope", "scope");
  }

  [Fact]
  public void Namespace_ReturnsNull_WhenKeychainHasNoNamespaceSpecified() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("foo bar.baz qux");

    keyChainResult.Value.Namespace.Should().BeNull();
  }

  [Fact]
  public void Namespace_ReturnsNamespace_WhenNamespaceIsPresentInKeychain() {
    var keyChainResult = AuthorizationKeyChain.CreateFrom("ns:foo ns:bar.baz ns:qux");

    keyChainResult.Value.Namespace.Should().Be("ns");
  }
}