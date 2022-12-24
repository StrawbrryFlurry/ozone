using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence.Security.Configurations;

public sealed class OzoneRoleConfiguration : IEntityTypeConfiguration<OzoneRole> {
  public void Configure(EntityTypeBuilder<OzoneRole> builder) {
    builder.HasKey(x => x.Id);
  }
}