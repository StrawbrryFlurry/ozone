namespace Ozone.Common.Extensions;

public static class DictionaryExtensions {
  public static TElement GetOrAddThreadUnsafe<TKey, TElement>(
    this IDictionary<TKey, TElement> dictionary,
    TKey key,
    Func<TElement> createFallbackValue
  ) {
    if (dictionary.TryGetValue(key, out var value)) {
      return value;
    }

    var fallbackValue = createFallbackValue();
    dictionary.Add(key, fallbackValue);
    return fallbackValue;
  }
}