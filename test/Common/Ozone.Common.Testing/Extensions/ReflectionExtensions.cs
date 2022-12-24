using System.Linq.Expressions;
using System.Reflection;
using System.Security;

namespace Ozone.Testing.Common.Extensions;

public static class ReflectionExtensions {
  /// <summary>
  /// Sets the value of a property without a setter.
  /// <b>This is highly unsafe and should only be used in testing scenarios.</b>
  /// </summary>
  public static T SetPropertyBackingField<T, TR>(
    this T obj,
    Expression<MemberExpressionFunc<T, TR>> propertyExpression,
    TR value
  ) {
    var propertyName = propertyExpression.GetMemberExpressionName();
    var backingFieldName = GetBackingFieldName(propertyName);

    var backingField = GetBackingField<T>(backingFieldName);

    if (backingField is null) {
      backingField = GetNonCompilerGeneratedPropertyBackingField<T>(propertyName);
    }

    if (backingField is null) {
      throw new ArgumentException($"Could not resolve backing field for property {propertyName} in {typeof(T).Name}."
      );
    }

    backingField?.SetValue(obj, value);
    return obj;
  }

  private static FieldInfo? GetBackingField<T>(string backingFieldName) {
    var type = typeof(T);
    var backingField = typeof(T).GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
    while (backingField is null && type.BaseType is not null) {
      type = type.BaseType;
      backingField = type.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    return backingField;
  }

  private static string GetBackingFieldName(string propertyName) {
    return $"<{propertyName}>k__BackingField";
  }

  /// <summary>
  /// Tries to find the backing field for a property that has a user generated backing field.
  /// E.g. <code>public foo { get { return _foo; } }</code>
  /// </summary>
  private static FieldInfo? GetNonCompilerGeneratedPropertyBackingField<TObject>(string propertyName) {
    var property = typeof(TObject).GetProperty(propertyName)!;

    var propertyGetterMethod = property.GetGetMethod(true)!;
    var getterMethodILBytes = propertyGetterMethod.GetMethodBody()!.GetILAsByteArray()!;

    // This method only supports properties that have
    // a single return statement for the backing field.
    if (getterMethodILBytes.Length != 7) {
      return null;
    }

    // The method has 7 bytes, so we can safely assume that the
    // The IL layout for such a method is:
    // 0x02: ldarg.0, 1 byte 
    // 0x7B: ldfld, 1 byte
    // int32: <field token>, 4 bytes
    // 0x2A: ret, 1 byte
    // See: https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLRII5wJZICaoC2EhwECyANDCNgDYA+AAgEwCMAsAFBMDMABK34BhfgG9u/KfwAOCbADdYEftgB2MfgH0wAe10BuSdL6qN/AGL7+AXgB82vYe4BfIA
    // And can extract the field token from the byte array.
    var fieldTokenBytes = getterMethodILBytes[2..6];
    var backingFieldToken = BitConverter.ToInt32(fieldTokenBytes);

    try {
      return property.Module.ResolveField(backingFieldToken)!;
    }
    catch {
      return null;
    }
  }
}