using System.Reflection;
using System.Security.Cryptography;
using Azure.Security.KeyVault.Keys;

namespace Ozone.Identity.Infrastructure.Tests.Unit.Security.Signing;

public sealed class KeyVaultKeyStub {
  private static readonly MethodInfo KeyPropertiesSetNameMethod =
    typeof(KeyProperties)
      .GetProperty(nameof(KeyProperties.Name))!
      .GetSetMethod(true)!;

  private static readonly MethodInfo KeyPropertiesSetVersionMethod =
    typeof(KeyProperties)
      .GetProperty(nameof(KeyProperties.Version))!
      .GetSetMethod(true)!;

  private static readonly MethodInfo KeyVaultKeySetJwkMethod =
    typeof(KeyVaultKey)
      .GetProperty(nameof(KeyVaultKey.Key))!
      .GetSetMethod(true)!;

  public static KeyVaultKey Create(string name, string version) {
    var key = new KeyVaultKey("https://oz-test.io/key");
    KeyPropertiesSetNameMethod.Invoke(key.Properties, new[] { name });
    KeyPropertiesSetVersionMethod.Invoke(key.Properties, new[] { version });
    key.SetExpiresOn(DateTimeOffset.MaxValue);
    key.SetEnabled(true);

    return key;
  }

  public static KeyVaultKey Create(string name, string version, byte[] rsaModulus) {
    var key = Create(name, version);
    var jwk = MakeJwk();
    jwk.N = rsaModulus;
    jwk.E = new byte[1] { 2 };
    jwk.KeyType = KeyType.Rsa;

    KeyVaultKeySetJwkMethod.Invoke(key, new[] { jwk });
    return key;
  }

  public static KeyVaultKey CreateWithRsa(string name, string version, int modulusSize = 256) {
    var key = Create(name, version);
    var jwk = MakeJwk();

    var modulus = new byte[modulusSize + 1];
    modulus[1] = 1; // Bypass null check

    jwk.N = modulus;
    jwk.E = new byte[1] { 1 };
    jwk.KeyType = KeyType.Rsa;

    KeyVaultKeySetJwkMethod.Invoke(key, new object?[] { jwk });

    return key;
  }

  public static KeyVaultKey CreateWithEc(string name, string version, int curveSize = 256) {
    var key = Create(name, version);
    var jwk = MakeJwk();

    var coordinate = Math.Ceiling(curveSize / 8.0);
    var xCoordinate = new byte[(int)coordinate];
    xCoordinate[1] = 1; // Bypass null check

    jwk.CurveName = $"P-{curveSize}";
    jwk.X = xCoordinate;
    jwk.Y = new byte[1] { 1 };
    jwk.KeyType = KeyType.Ec;

    KeyVaultKeySetJwkMethod.Invoke(key, new object?[] { jwk });

    return key;
  }

  private static JsonWebKey MakeJwk() {
    return (JsonWebKey)Activator.CreateInstance(typeof(JsonWebKey), true)!;
  }
}

public static class KeyVaultKeyExtensions {
  private static readonly MethodInfo KeyVaultKeySetEnabledMethod =
    typeof(KeyProperties)
      .GetProperty(nameof(KeyProperties.Enabled))!
      .GetSetMethod(true)!;

  private static readonly MethodInfo KeyPropertiesSetExpiresOnMethod =
    typeof(KeyProperties)
      .GetProperty(nameof(KeyProperties.ExpiresOn))!
      .GetSetMethod(true)!;


  public static void SetExpiresOn(this KeyVaultKey key, DateTimeOffset expiresOn) {
    KeyPropertiesSetExpiresOnMethod.Invoke(key.Properties, new object?[] { expiresOn });
  }

  public static void SetEnabled(this KeyVaultKey key, bool enabled) {
    KeyVaultKeySetEnabledMethod.Invoke(key.Properties, new object?[] { enabled });
  }
}