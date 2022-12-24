namespace Ozone.Common.Domain.Data;

/// <summary>
/// Represents the persistence object in this application.
/// When using EF this would be the apps DbContext. 
/// </summary>
public interface IUnitOfWork {
  /// <summary>
  /// Commits the changes made to persistence.
  /// </summary>
  /// <param name="ct"></param>
  /// <returns></returns>
  public Task CommitAsync(CancellationToken ct = default);
}