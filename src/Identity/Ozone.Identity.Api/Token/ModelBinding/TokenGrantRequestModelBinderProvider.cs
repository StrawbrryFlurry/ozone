using Microsoft.AspNetCore.Mvc.ModelBinding;
using Ozone.Identity.Api.Token.Contracts;
using Ozone.Identity.Api.Token.Contracts.GrantTypes;

namespace Ozone.Identity.Api.Token.ModelBinding;

public sealed class TokenGrantRequestModelBinderProvider : IModelBinderProvider {
  private static readonly Dictionary<string, Type> ModelMetadata = GetModelMetadata();

  public IModelBinder? GetBinder(ModelBinderProviderContext context) {
    if (context.Metadata.ModelType != typeof(TokenGrantRequest)) {
      return null;
    }

    var binders = new Dictionary<string, (ModelMetadata Metadata, IModelBinder Binder)>();

    foreach (var (grantType, modelType) in ModelMetadata) {
      var modelMetadata = context.MetadataProvider.GetMetadataForType(modelType);
      var binder = context.CreateBinder(modelMetadata);
      binders[grantType] = (modelMetadata, binder);
    }

    return new TokenGrantRequestModelBinder(binders);
  }

  private static Dictionary<string, Type> GetModelMetadata() {
    // We could use reflection to find all the types that implement ITokenGrantRequest
    // but for the sake of simplicity we'll just hard code them here.
    return new Dictionary<string, Type> {
      { "code", typeof(AuthorizationCodeTokenGrantRequest) },
      { "refresh_token", typeof(RefreshTokenTokenGrantRequest) }
    };
  }
}