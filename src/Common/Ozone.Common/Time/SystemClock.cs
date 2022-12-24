namespace Ozone.Common.Time;

public sealed class SystemClock : ISystemClock {
  public static readonly ISystemClock Clock = new SystemClock();
  public DateTimeOffset UtcNow => DateTimeOffset.Now;
}