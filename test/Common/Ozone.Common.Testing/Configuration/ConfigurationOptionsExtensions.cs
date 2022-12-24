using Microsoft.Extensions.Options;

namespace Ozone.Testing.Common.Configuration;

public static class ConfigurationOptionsExtensions {
  public static IOptions<T> ToOptions<T>(this T options) where T : class {
    return new OptionsWrapper<T>(options);
  }

  public static IOptionsMonitor<T> ToOptionsMonitor<T>(this T options) where T : class {
    return new ChangeableOptionsMonitor<T>(options);
  }

  public static void ChangeOption<T>(this IOptionsMonitor<T> monitor, T newOptions) {
    if (monitor is not ChangeableOptionsMonitor<T> ct) {
      throw new InvalidOperationException("ChangeOption is only valid on an option created with ToOptionsMonitor");
    }

    ct.Change(newOptions);
  }

  public sealed class ChangeableOptionsMonitor<T> : IOptionsMonitor<T> {
    public T Get(string? name) {
      return CurrentValue;
    }

    public ChangeableOptionsMonitor(T currentValue) {
      CurrentValue = currentValue;
    }

    public IDisposable? OnChange(Action<T, string?> listener) {
      throw new NotImplementedException();
    }

    public T CurrentValue { get; private set; }

    public void Change(T newOptions) {
      CurrentValue = newOptions;
    }
  }
}