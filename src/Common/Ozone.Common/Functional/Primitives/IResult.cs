namespace Ozone.Common.Functional;

public interface IResult {
  public bool IsSuccess { get; }
  public bool IsFailure { get; }
  public Error Error { get; }
}