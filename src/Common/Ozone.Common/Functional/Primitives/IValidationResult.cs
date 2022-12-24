namespace Ozone.Common.Functional;

public interface IValidationResult : IResult {
  public Error[] Errors { get; }
}