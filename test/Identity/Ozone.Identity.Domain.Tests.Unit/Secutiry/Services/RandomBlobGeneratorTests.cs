using FluentAssertions;
using Ozone.Identity.Domain.Security.Services;

namespace Ozone.Identity.Domain.Tests.Unit.Secutiry.Services;

public sealed class RandomBlobGeneratorTests {
  [Fact]
  public void GenerateRandomBlob_ReturnsBlobOfSpecifiedLength() {
    var blob = RandomBlobGenerator.GenerateString(128);
    blob.Should().NotBeEmpty();
  }
}