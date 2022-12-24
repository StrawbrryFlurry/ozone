using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ozone.Common.Functional;

public readonly struct Error : IError, IEquatable<Error> {
  public static readonly Error None = new("", "");
  public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

  public static bool ArgumentNullOrEmpty(
    string? argument,
    out Error error,
    [CallerArgumentExpression("argument")] string? argumentName = null
  ) {
    if (string.IsNullOrWhiteSpace(argument)) {
      error = new Error("Error.NullValue", $"Argument {argumentName} cannot be null or empty");
      return true;
    }

    error = default;
    return false;
  }

  public static Error ArgumentOutOfRange(
    object? argument,
    string reason = "",
    [CallerArgumentExpression("argument")] string? argumentName = null
  ) {
    return new Error("Error.ArgumentOutOfRange", $"{argumentName} was out of range. {reason}");
  }

  public Error() { }

  [SetsRequiredMembers]
  public Error(string code, string message, Exception? cause = null) {
    Code = code;
    Message = message;
    Cause = cause;
  }

  public required string Code { get; init; }

  public required string Message { get; init; }

  public Exception? Cause { get; init; }

  public static implicit operator string(Error error) {
    return error.Code;
  }

  public static bool operator ==(Error? a, Error? b) {
    if (a is null && b is null) {
      return true;
    }

    if (a is null || b is null) {
      return false;
    }

    return a.Equals(b);
  }

  public static bool operator !=(Error? a, Error? b) {
    return !(a == b);
  }

  public bool Equals(Error? other) {
    if (other is null) {
      return false;
    }

    return Equals(other.Value);
  }


  public override bool Equals(object? obj) {
    return obj is Error other && Equals(other);
  }

  public override int GetHashCode() {
    return HashCode.Combine(Code, Message, Cause);
  }

  public override string ToString() {
    return Code;
  }

  public bool Equals(Error other) {
    return Code == other.Code && Message == other.Message && Equals(Cause, other.Cause);
  }
}