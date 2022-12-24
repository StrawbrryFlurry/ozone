using Ozone.Common.Identification;

namespace Ozone.Common.Domain.Data;

/// <summary>
///   Entities implementing this interface are
///   tracked by a <see cref="CorrelationId" />,
///   pinning them to a certain transaction or
///   event that caused them to be created.
/// </summary>
public interface ICorrelatableEntity {
  /// <summary>
  ///   The ID of the operation that this entity
  ///   belongs to.
  /// </summary>
  public CorrelationId CorrelationId { get; }
}