using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Ozone.Common.Domain.Data;
using Ozone.Identity.Domain.Auth;
using Ozone.Identity.Domain.EnterpriseApplications;
using Ozone.Identity.Domain.Security;

namespace Ozone.Identity.Persistence;

public sealed class IdentityContext : DbContext, IUnitOfWork {
  public const string DefaultSchema = "identity";

  public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
  public DbSet<Scope> Scopes { get; set; } = null!;
  public DbSet<ExternalAuthenticationChallenge> ExternalAuthenticationChallenges { get; set; } = null!;
  public DbSet<EnterpriseApplication> EnterpriseApplications { get; set; } = null!;
  public DbSet<ServiceApplication> ServiceApplications { get; set; } = null!;
  public DbSet<AuthorizationCode> AuthenticationCodes { get; set; } = null!;

  public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder builder) {
    builder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
  }

  public Task CommitAsync(CancellationToken ct = default) {
    return base.SaveChangesAsync(ct);
  }
}