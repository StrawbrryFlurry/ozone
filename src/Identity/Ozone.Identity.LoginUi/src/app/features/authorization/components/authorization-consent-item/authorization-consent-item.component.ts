import {
  ChangeDetectionStrategy,
  Component,
  Input,
  OnInit,
} from '@angular/core';

@Component({
  selector: 'oz-identity-authorization-consent-item',
  templateUrl: './authorization-consent-item.component.html',
  styleUrls: ['./authorization-consent-item.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthorizationConsentItemComponent {
  @Input() public name!: string;
  @Input() public description!: string;
}
