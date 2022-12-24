using Ozone.Common.Time;

namespace Ozone.Testing.Common.Time;

public sealed class StaticTimeSystemClock : ISystemClock {
  public static StaticTimeSystemClock Instance { get; } = new(TimeUtils.TestTime);

  public StaticTimeSystemClock(DateTimeOffset now) {
    UtcNow = now;
  }

  public DateTimeOffset UtcNow { get; }
}