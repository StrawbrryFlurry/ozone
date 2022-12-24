namespace Ozone.Common.Functional;

public interface IResult<out TValue> : IResult {
  public TValue Value { get; }
}