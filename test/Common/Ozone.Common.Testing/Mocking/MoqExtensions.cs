using System.Linq.Expressions;
using System.Reflection;
using Moq;
using Ozone.Testing.Common.Extensions;

namespace Ozone.Testing.Common.Mocking;

public static class MoqExtensions {
  public static IEnumerable<IInvocation> InvocationsOfName<T>(this Mock<T> mock, string methodName) where T : class {
    return mock.Invocations.Where(x => x.Method.Name == methodName);
  }

  public static IEnumerable<IInvocation> InvocationsOfNames<T>(this Mock<T> mock, params string[] methodNames)
    where T : class {
    return mock.Invocations.Where(i => methodNames.Contains(i.Method.Name));
  }

  public static IInvocation FirstInvocationOfName<T>(
    this Mock<T> mock,
    Expression<MemberExpressionFunc<T, Delegate>> memberExpression
  )
    where T : class {
    var memberName = memberExpression.GetMemberExpressionName();
    return mock.Invocations.First(x => x.Method.Name == memberName);
  }

  public static T GetArgument<T>(this IInvocation invocation, int argumentIndex = 0) {
    return (T)invocation.Arguments[argumentIndex];
  }
}