namespace Ozone.Common.Domain.Data;

/// <summary>
///   Represents a base Entity in the domain
/// </summary>
public abstract class Entity : Entity<Guid> {
  protected Entity() : base(Guid.NewGuid()) { }
  protected Entity(Guid guid) : base(guid) { }
}

public abstract class Entity<TUniqueId> : IEquatable<Entity<TUniqueId>> {
  protected Entity(TUniqueId id) {
    Id = id;
  }

  protected Entity() {
    Id = default;
  }

  public TUniqueId Id { get; }

  public bool Equals(Entity<TUniqueId>? other) {
    if (other is null) {
      return false;
    }

    if (other.GetType() != GetType()) {
      return false;
    }

    return other.Id?.Equals(Id) ?? false;
  }

  /// <summary>
  ///   Indicates whether the entity is not persisted.
  /// </summary>
  /// <returns></returns>
  public bool IsTransient() {
    return Id?.Equals(default) ?? true;
  }

  public static bool operator ==(Entity<TUniqueId>? first, Entity<TUniqueId>? second) {
    return first is not null && second is not null && first.Equals(second);
  }

  public static bool operator !=(Entity<TUniqueId>? first, Entity<TUniqueId>? second) {
    return !(first == second);
  }

  public override bool Equals(object? obj) {
    if (obj is null) {
      return false;
    }

    if (obj.GetType() != GetType()) {
      return false;
    }

    if (obj is not Entity<TUniqueId> entity) {
      return false;
    }

    if (entity.IsTransient() || IsTransient()) {
      return false;
    }

    return entity.Id?.Equals(Id) ?? false;
  }

  public override int GetHashCode() {
    return Id.GetHashCode();
  }
}