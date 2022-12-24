using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Security.Jwt;

public abstract class JwtTokenSegment : Dictionary<string, object?> {
  public T? GetOptionalEntry<T>(string name) {
    if (!TryGetValue(name, out var value)) {
      return default;
    }

    if (value is T v) {
      return v;
    }

    ;

    return default;
  }

  public T GetRequiredEntryFormat<T>(string name, Func<object, T> formatter) {
    if (!TryGetValue(name, out var value)) {
      throw new InvalidOperationException($"Required entry {name} is missing in jwt");
    }

    if (value is T v) {
      return v;
    }

    var formatted = formatter.Invoke(value);
    this[name] = formatted;
    return formatted;
  }

  public T? GetRequiredEntry<T>(string name) {
    if (!TryGetValue(name, out var value)) {
      throw new InvalidOperationException($"Required entry {name} is missing in jwt");
    }

    if (value is T v) {
      return v;
    }

    // If we parse an existing jwt,
    // dates will be present as a unix timestamp
    if (TryParseUnixTimestamp<T>(value, out var timestamp)) {
      return timestamp!;
    }

    return default;
  }

  private bool TryParseUnixTimestamp<T>(object? value, out T? parsed) {
    if (typeof(T) != typeof(DateTimeOffset)) {
      parsed = default;
      return false;
    }

    if (value is not long l) {
      parsed = default;
      return false;
    }

    parsed = (T)(object)DateTimeOffset.FromUnixTimeSeconds(l);
    return true;
  }

  public string ToBase64Encoded() {
    return ToSerializedJson().ToBase64UrlEncoded();
  }

  public string ToSerializedJson(Formatting formatting = Formatting.None) {
    // All date time references in a JWT need to be in form of a Unix TimeStamp
    return JsonConvert.SerializeObject(this, formatting, new SigningAlgorithmJsonConverter(),
      new UnixDateTimeConverter());
  }

  /// <summary>
  /// Allows users to add or set an element in the dictionary.
  /// This is useful when a segment has certain keys that are added
  /// by default and then using the collection initializer to add new
  /// elements to that segment. 
  /// </summary>
  /// <param name="key"></param>
  /// <param name="value"></param>
  internal new void Add(string key, object value) {
    if (!TryAdd(key, value)) {
      this[key] = value;
      return;
    }

    if (this[key] is not null) {
      return;
    }

    this[key] = value;
  }
}