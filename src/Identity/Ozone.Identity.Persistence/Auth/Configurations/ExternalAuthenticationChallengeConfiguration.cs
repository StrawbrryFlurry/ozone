using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Auth;

namespace Ozone.Identity.Persistence.Auth.Configurations;

public sealed class ExternalAuthenticationChallengeConfiguration : IEntityTypeConfiguration<ExternalAuthenticationChallenge> {
  public void Configure(EntityTypeBuilder<ExternalAuthenticationChallenge> builder) {
    builder.ToTable(IdentityTableNames.ExternalAuthenticationChallenge, IdentityContext.DefaultSchema);

    builder.HasKey(x => x.Id);
    builder.OwnsOne(x => x.ResponseMode);
    builder.OwnsOne(x => x.CodeChallengeMode);
    builder.OwnsOne(x => x.IdpCodeChallengeMode);
  }
}