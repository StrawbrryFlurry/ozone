namespace Ozone.Common.Core.Abstractions;

public sealed class ParameterCollection : Dictionary<string, string> {
  public ParameterCollection() { }
  public ParameterCollection(IEnumerable<KeyValuePair<string, string>> parameters) : base(parameters) { }
}