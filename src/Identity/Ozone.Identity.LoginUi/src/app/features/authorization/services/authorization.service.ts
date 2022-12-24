import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IAuthorizationConsentRequest } from '../interfaces/authorization-consent-request.interface';
import { IIAuthorizationRequest } from '../interfaces/authorization-request.interface';

@Injectable()
export class AuthorizationService {
  constructor(private _httpClient: HttpClient) {}

  public async forbid(request: IIAuthorizationRequest): Promise<URL> {
    return this._buildErrorUri(
      request,
      'Identity.Authorization.ConsentForbidden',
      'The user did not consent to the authorization request.'
    );
  }

  public async consent(request: IIAuthorizationRequest): Promise<URL> {
    const consentRequest: IAuthorizationConsentRequest = {
      authorizationCode: request.code,
    };

    try {
      await firstValueFrom(
        this._httpClient.post(
          environment.authorization.consentUrl,
          consentRequest
        )
      );
    } catch {
      return this._buildErrorUri(
        request,
        'Identity.Authorization.ConsentFailed',
        'An error occurred while saving the user consent. Try again at a later time.'
      );
    }

    return this._buildConsentUrl(request);
  }

  private _buildConsentUrl(request: IIAuthorizationRequest) {
    const url = new URL(request.redirectUri);
    url.searchParams.set('code', request.code);
    url.searchParams.set('state', request.state);

    return url;
  }

  private _buildErrorUri(
    request: IIAuthorizationRequest,
    error: string,
    errorDescription: string
  ): URL {
    const url = new URL(request.redirectUri);
    url.searchParams.set('error', error);
    url.searchParams.set('error_description', errorDescription);
    url.searchParams.set('state', request.state);

    return url;
  }
}
