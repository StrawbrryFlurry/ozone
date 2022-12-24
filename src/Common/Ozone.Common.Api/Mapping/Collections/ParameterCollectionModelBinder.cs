using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Ozone.Common.Core.Abstractions;

namespace Ozone.Common.Api.Mapping.Collections;

public sealed class ParameterCollectionModelBinder : IModelBinder {
  public Task BindModelAsync(ModelBindingContext bindingContext) {
    if (bindingContext == null) {
      throw new ArgumentNullException(nameof(bindingContext));
    }

    var modelName = bindingContext.ModelName;

    var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
    if (valueProviderResult == ValueProviderResult.None) {
      return Task.CompletedTask;
    }

    bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

    var value = valueProviderResult.FirstValue;

    // Check if the argument value is null or empty
    if (string.IsNullOrEmpty(value)) {
      return Task.CompletedTask;
    }

    try {
      var result = JsonConvert.DeserializeObject<ParameterCollection>(value);
      bindingContext.Result = ModelBindingResult.Success(result);
      return Task.CompletedTask;
    }
    catch (Exception ex) {
      bindingContext.ModelState.TryAddModelError(modelName, ex, bindingContext.ModelMetadata);
      return Task.CompletedTask;
    }
  }
}