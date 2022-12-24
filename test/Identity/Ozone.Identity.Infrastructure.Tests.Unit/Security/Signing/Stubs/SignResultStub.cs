using System.Reflection;
using Azure;
using Azure.Security.KeyVault.Keys.Cryptography;
using static System.Reflection.BindingFlags;

namespace Ozone.Identity.Infrastructure.Tests.Unit.Security.Signing;

public sealed class SignResultStub {
  private static readonly ConstructorInfo SignResultCtor =
    typeof(SignResult).GetConstructor(NonPublic | Instance | CreateInstance, Type.EmptyTypes)!;

  private static readonly PropertyInfo SignResultSignatureProperty =
    typeof(SignResult).GetProperty(nameof(SignResult.Signature))!;

  private static readonly MethodInfo SignResultSetSignatureMethod =
    SignResultSignatureProperty.GetSetMethod(true)!;

  public static SignResult Create(byte[]? signature = null) {
    signature ??= new byte [256];
    var signResult = (SignResult)SignResultCtor.Invoke(Array.Empty<object>())!;
    SignResultSetSignatureMethod.Invoke(signResult, new object[] { signature });
    return signResult;
  }
}