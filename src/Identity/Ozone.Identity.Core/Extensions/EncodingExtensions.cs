using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ozone.Identity.Core.Extensions;

public static class EncodingExtensions {
  public static string ToUtf8String(this byte[] value) {
    return Encoding.UTF8.GetString(value);
  }

  public static string ToBase64UrlEncoded(this byte[] value) {
    return Base64UrlEncoder.Encode(value);
  }

  public static string ToBase64UrlEncoded(this string value) {
    return Base64UrlEncoder.Encode(value);
  }

  public static string FromBase64UrlEncoded(this string value) {
    return Base64UrlEncoder.Decode(value);
  }

  public static byte[] GetUtf8Bytes(this string value) {
    return Encoding.UTF8.GetBytes(value);
  }
}