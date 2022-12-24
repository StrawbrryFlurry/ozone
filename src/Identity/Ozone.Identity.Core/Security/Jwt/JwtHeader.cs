using Newtonsoft.Json;
using Ozone.Common.Functional;
using Ozone.Common.Functional.Extensions;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Signing;

namespace Ozone.Identity.Core.Security.Jwt;

public struct JwtHeaderField {
  public const string Alg = "alg";
  public const string Kid = "kid";
  public const string Typ = "typ";
}

public sealed class JwtHeader : JwtTokenSegment {
  public JwtHeader() : this("") { }

  public JwtHeader(string keyId) : this(keyId, SigningAlgorithm.Default) { }

  public JwtHeader(string keyId, SigningAlgorithm? algorithm = null) {
    Add(JwtHeaderField.Typ, "JWT");
    Add(JwtHeaderField.Kid, keyId);
    Add(JwtHeaderField.Alg, algorithm ?? SigningAlgorithm.Default);
  }

  public SigningAlgorithm? Algorithm => GetRequiredEntryFormat(
    JwtHeaderField.Alg,
    o => {
      return o switch {
        string s => SigningAlgorithm.Parse(s).OnFailureCompensate(default).Value,
        SigningAlgorithm sa => sa,
        _ => default
      };
    });

  public string? SigningKey => GetRequiredEntry<string>(JwtHeaderField.Kid);
  public string? Type => GetOptionalEntry<string>(JwtHeaderField.Typ);

  public static Result<JwtHeader> FromBase64Header(string header) {
    try {
      return JsonConvert.DeserializeObject<JwtHeader>(header.FromBase64UrlEncoded())!;
    }
    catch (Exception exception) {
      return new Error("Identity.Sign", "Invalid jwt header", exception);
    }
  }
}