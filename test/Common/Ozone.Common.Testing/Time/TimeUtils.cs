using System.Diagnostics.CodeAnalysis;
using Ozone.Common.Time;

namespace Ozone.Testing.Common.Time;

public static class TimeUtils {
  public static DateTimeOffset TestTime = DateTimeOffset.Parse("2000-10-16T00:00:00.0000000Z");

  public static ISystemClock TestClockAt([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string date) {
    return new StaticTimeSystemClock(DateTimeOffset.Parse(date));
  }

  public static ISystemClock TestClock() {
    return new StaticTimeSystemClock(TestTime);
  }
}