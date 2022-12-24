import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  OnInit,
  Output,
} from '@angular/core';

@Component({
  selector: 'oz-identity-authorization-consent-buttons',
  templateUrl: './authorization-consent-buttons.component.html',
  styleUrls: ['./authorization-consent-buttons.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthorizationConsentButtonsComponent {
  @Output() public consent = new EventEmitter();
  @Output() public forbid = new EventEmitter();

  public onConsent() {
    this.consent.emit();
  }

  public onForbid() {
    this.forbid.emit();
  }
}
