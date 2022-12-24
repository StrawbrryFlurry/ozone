import { DOCUMENT, Location } from '@angular/common';
import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, map, Observable } from 'rxjs';
import { isNil } from 'src/app/shared/utils';
import {
  IAuthorizationScope,
  IIAuthorizationRequest,
} from '../../interfaces/authorization-request.interface';
import { AuthorizationService } from '../../services/authorization.service';

// Test Uri: http://localhost:4200/authorize?client_name=Test%20Application&client_description=Some%20Test%20Application&username=Test%20User&code=AZlVrRggCnoWmQblQAhZiNgQ2Tnn5bNDahNcZvDQlVALOo36oRtSnZbHpKa5ltEwH4s1S7AMgNdfpDUdP3KlkUxtKWZV638DpMjfSuAosa16PYpZdNECBXMYaQWHSokviHT9XlvwQAHizex0I6IoFG1bJ9eX7Vo8u2F6dPyipHQi&scopes=%5B%7B%22name%22:%22service.action.2%22,%22description%22:%22Test%20Action%202%22%7D,%7B%22name%22:%22openid%22,%22description%22:%22Read%20your%20basic%20profile%22%7D,%7B%22name%22:%22Test.Scope%22,%22description%22:%22Test%20scope%22%7D%5D&redirect_uri=https:%2F%2Fclient-app.redirect%2F&state=1234

@Component({
  selector: 'oz-identity-authorization',
  templateUrl: './authorization.page.html',
  styleUrls: ['./authorization.page.scss'],
})
export class AuthorizationPage {
  public authorizationRequest: Observable<IIAuthorizationRequest>;
  public authorizationScopes!: IAuthorizationScope[];

  constructor(
    private _route: ActivatedRoute,
    @Inject(DOCUMENT) private _document: Document,
    private _authorizationService: AuthorizationService
  ) {
    this.authorizationRequest = this._route.data.pipe(
      filter((data) => !isNil(data['authorizationRequest'])),
      map((data) => data['authorizationRequest'])
    );
  }

  public async onConsent(request: IIAuthorizationRequest) {
    const redirectUri = await this._authorizationService.consent(request);
    this._document.location.assign(redirectUri);
  }

  public async onForbid(request: IIAuthorizationRequest) {
    const redirectUri = await this._authorizationService.forbid(request);
    this._document.location.assign(redirectUri);
  }
}
