import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
} from '@angular/core';
import { IAuthorizationScope } from '../../interfaces/authorization-request.interface';

@Component({
  selector: 'oz-identity-authorization-consent-list',
  templateUrl: './authorization-consent-list.component.html',
  styleUrls: ['./authorization-consent-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthorizationConsentListComponent {
  @Input() public authorizationScopes!: IAuthorizationScope[];
}
