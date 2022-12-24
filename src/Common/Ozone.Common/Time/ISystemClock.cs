namespace Ozone.Common.Time;

public interface ISystemClock {
  public DateTimeOffset UtcNow { get; }
}