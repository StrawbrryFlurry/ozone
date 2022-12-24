namespace Ozone.Common.Extensions;

public static class EnumerableExtensions {
  public static string JoinBy<T>(this IEnumerable<T> source, string separator) {
    return string.Join(separator, source);
  }
}