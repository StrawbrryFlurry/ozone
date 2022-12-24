namespace Ozone.Common.Domain.Data;

/// <summary>
/// Represents a base Value Object in the domain. See
/// <a href="https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects">Value Object</a>
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject> {
  public static bool operator ==(ValueObject? left, ValueObject? right) {
    if (left is null || right is null) {
      return false;
    }

    return ReferenceEquals(left, right) || left.Equals(right);
  }

  public static bool operator !=(ValueObject? left, ValueObject? right) {
    return !(left == right);
  }

  protected abstract IEnumerable<object?> GetEqualityComponents();

  bool IEquatable<ValueObject>.Equals(ValueObject? other) {
    return Equals(other);
  }

  public override bool Equals(object? obj) {
    if (ReferenceEquals(null, obj)) {
      return false;
    }

    if (ReferenceEquals(this, obj)) {
      return true;
    }

    if (obj.GetType() != GetType()) {
      return false;
    }

    return ValuesAreEqual((ValueObject)obj);
  }

  private bool ValuesAreEqual(ValueObject valueObject) {
    return GetEqualityComponents().SequenceEqual(valueObject!.GetEqualityComponents());
  }

  public override int GetHashCode() {
    return GetEqualityComponents()
      .Select(x => x?.GetHashCode() ?? 0)
      .Aggregate((x, y) => x ^ y);
  }
}