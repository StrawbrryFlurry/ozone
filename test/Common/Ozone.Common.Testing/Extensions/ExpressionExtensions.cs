using System.Linq.Expressions;
using System.Reflection;

namespace Ozone.Testing.Common.Extensions;

public delegate TMember MemberExpressionFunc<in T, out TMember>(T obj);

public static class ExpressionExtensions {
  public static string
    GetMemberExpressionName<T, TMember>(this Expression<MemberExpressionFunc<T, TMember>> expression) {
    var memberExpression = expression.Body as MemberExpression;

    if (memberExpression is not null) {
      return memberExpression.Member.Name;
    }

    if (expression.Body is not UnaryExpression unaryExpression) {
      throw new ArgumentException("Expression is not a member access", nameof(expression));
    }

    if (unaryExpression.Operand is MemberExpression operandMemberExpression) {
      return operandMemberExpression.Member.Name;
    }

    if (unaryExpression.Operand is not MethodCallExpression convertMethodCall) {
      throw new ArgumentException("Expression is not a member access", nameof(expression));
    }

    var targetObj = convertMethodCall.Object as ConstantExpression;

    if (targetObj?.Value is not MemberInfo memberInfo) {
      throw new ArgumentException("Expression is not a member access", nameof(expression));
    }

    return memberInfo.Name;
  }
}