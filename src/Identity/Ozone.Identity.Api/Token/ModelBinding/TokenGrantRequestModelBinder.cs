using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ozone.Identity.Api.Token.Contracts;

namespace Ozone.Identity.Api.Token.ModelBinding;

public sealed class TokenGrantRequestModelBinder : IModelBinder {
  private const string GrantTypeKey = TokenGrantRequest.GrantTypeField;

  private readonly Dictionary<string, (ModelMetadata Metadata, IModelBinder Binder)> _binders;

  public TokenGrantRequestModelBinder(Dictionary<string, (ModelMetadata metadata, IModelBinder binder)> binders) {
    _binders = binders;
  }

  public async Task BindModelAsync(ModelBindingContext? bindingContext) {
    if (bindingContext == null) {
      throw new ArgumentNullException(nameof(bindingContext));
    }

    var grantType = GetGrantType(bindingContext.ValueProvider);

    if (grantType is null) {
      bindingContext.ModelState.AddModelError(GrantTypeKey, $"The {GrantTypeKey} field is required.");
      bindingContext.Result = ModelBindingResult.Failed();
      return;
    }

    await BindGrantTypeModel(grantType, bindingContext);
  }

  private static string? GetGrantType(IValueProvider valueProvider) {
    return valueProvider.GetValue(GrantTypeKey).FirstValue;
  }

  private async Task BindGrantTypeModel(string grantType, ModelBindingContext baseBindingContext) {
    if (!_binders.TryGetValue(grantType, out var grantTypeBinder)) {
      baseBindingContext.ModelState.AddModelError(GrantTypeKey, $"The {GrantTypeKey} field is invalid.");
      baseBindingContext.Result = ModelBindingResult.Failed();
      return;
    }

    var (metadata, concreteBinder) = grantTypeBinder;
    var concreteBindingContext = DefaultModelBindingContext.CreateBindingContext(
      baseBindingContext.ActionContext,
      baseBindingContext.ValueProvider,
      metadata,
      null,
      baseBindingContext.ModelName
    );
    concreteBindingContext.BindingSource = baseBindingContext.BindingSource;

    await concreteBinder.BindModelAsync(concreteBindingContext);
    var bindingResult = concreteBindingContext.Result;
    baseBindingContext.Result = bindingResult;

    if (concreteBindingContext.Result.IsModelSet) {
      baseBindingContext.ValidationState[bindingResult.Model!] = new ValidationStateEntry {
        Metadata = metadata
      };
    }
  }
}