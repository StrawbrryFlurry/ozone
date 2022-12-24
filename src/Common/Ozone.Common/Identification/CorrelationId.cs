namespace Ozone.Common.Identification;

public readonly struct CorrelationId {
  public CorrelationId(Guid guid) {
    Value = guid;
  }

  public CorrelationId() {
    Value = Guid.NewGuid();
  }

  public Guid Value { get; }

  public override string ToString() {
    return Value.ToString();
  }

  public static bool TryParse(string value, out CorrelationId result) {
    if (Guid.TryParse(value, out var guid)) {
      result = new CorrelationId(guid);
      return true;
    }

    result = default;
    return false;
  }

  public static implicit operator Guid(CorrelationId id) {
    return id.Value;
  }

  public static implicit operator CorrelationId(Guid id) {
    return new CorrelationId(id);
  }

  public static implicit operator string(CorrelationId id) {
    return id.Value.ToString();
  }
}