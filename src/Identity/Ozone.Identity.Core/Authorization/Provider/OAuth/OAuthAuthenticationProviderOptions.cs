namespace Ozone.Identity.Core.Authorization.Provider.OAuth;

public class OAuthAuthenticationProviderOptions : IAuthenticationProviderOptions {
  public string ClientId { get; set; }
  public string ClientSecret { get; set; }
  public string Authority { get; set; }

  /// <summary>
  ///   The base address of the API. This is used to construct the redirect URI.
  ///   Example: login.microsoft.com
  /// </summary>
  public string Instance { get; set; }

  /// <summary>
  /// The URI under which the identity provider
  /// is running, including the path to the authorize uri.
  /// E.g. https://identity.ozone.io/oauth2/v1/authorize
  /// </summary>
  public string AuthorizeUri { get; set; }

  /// <summary>
  /// The Uri under which the user is prompted to authorize
  /// the client application to access the user's data.
  /// </summary>
  public string OzoneAuthroizeUri { get; set; }
}