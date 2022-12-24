import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
} from '@angular/core';

@Component({
  selector: 'oz-identity-authorization-consent-info',
  templateUrl: './authorization-consent-info.component.html',
  styleUrls: ['./authorization-consent-info.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthorizationConsentInfoComponent {
  @Input() public username!: string;
  @Input() public applicationName!: string;
  @Input() public applicationDescription!: string;
}
