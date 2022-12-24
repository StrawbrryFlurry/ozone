namespace Ozone.Common.Functional;

public interface IError {
  public string Code { get; init; }
  public string Message { get; init; }
}