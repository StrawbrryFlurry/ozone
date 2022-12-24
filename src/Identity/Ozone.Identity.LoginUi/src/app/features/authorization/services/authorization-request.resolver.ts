import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { Observable } from 'rxjs';
import { isNil } from 'src/app/shared/utils';
import {
  IAuthorizationScope,
  IIAuthorizationRequest,
} from '../interfaces/authorization-request.interface';

@Injectable({ providedIn: 'root' })
export class AuthorizationRequestResolver
  implements Resolve<IIAuthorizationRequest>
{
  constructor(private _router: Router) {}

  public resolve(
    route: ActivatedRouteSnapshot,
    routeState: RouterStateSnapshot
  ):
    | Observable<IIAuthorizationRequest>
    | Promise<IIAuthorizationRequest>
    | IIAuthorizationRequest {
    const code = route.queryParamMap.get('code');
    const redirectUri = route.queryParamMap.get('redirect_uri');
    const state = route.queryParamMap.get('state');
    const clientName = route.queryParamMap.get('client_name');
    const clientDescription =
      route.queryParamMap.get('client_description') ?? '';
    const username = route.queryParamMap.get('username');
    const scopes = route.queryParamMap.get('scopes');

    if (
      isNil(code) ||
      isNil(redirectUri) ||
      isNil(state) ||
      isNil(clientName) ||
      isNil(scopes) ||
      isNil(username)
    ) {
      return this.handleInvalidRequest();
    }

    const parsedScopes = this.parseScopes(scopes);

    if (isNil(parsedScopes)) {
      return this.handleInvalidRequest();
    }

    return {
      code,
      redirectUri,
      state,
      username,
      clientName,
      clientDescription,
      scopes: parsedScopes,
    };
  }

  private parseScopes(scopeString: string): IAuthorizationScope[] {
    const scopes = JSON.parse(scopeString) as IAuthorizationScope[];

    if (!Array.isArray(scopes)) {
      return null!;
    }

    for (const scope of scopes) {
      const { name, description } = scope;
      if (isNil(name) || isNil(description)) {
        return this.handleInvalidRequest();
      }
    }

    return scopes;
  }

  private handleInvalidRequest(): any /* We redirect here so the return type doesn't matter */ {
    this._router.navigate(['']);
    return null!;
  }
}
