using System.Security.Cryptography;

namespace Ozone.Identity.Domain.Security.Services;

public static class RandomBlobGenerator {
  /// <summary>
  /// Creates a random string blob containing characters
  /// from a-z, A-Z and 0-9.
  /// </summary>
  /// <param name="blobSizeInBytes"></param>
  /// <returns></returns>
  public static string GenerateString(int blobSizeInBytes) {
    var bytes = RandomNumberGenerator.GetBytes(blobSizeInBytes);
    var base64String = Convert.ToBase64String(bytes);
    return RemoveSpecialCharactersFromBase64String(base64String);
  }

  private static string RemoveSpecialCharactersFromBase64String(string base64String) {
    // We don't care about the input bytes so we can statically replace special
    // characters without worrying about losing the initial input bytes.
    return base64String.Replace("+", "o").Replace("/", "Z").Replace("=", "i");
  }
}