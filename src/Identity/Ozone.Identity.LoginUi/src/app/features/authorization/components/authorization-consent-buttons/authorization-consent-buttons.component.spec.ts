import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationConsentButtonsComponent } from './authorization-consent-buttons.component';

describe('AuthorizationConsentButtonsComponent', () => {
  let component: AuthorizationConsentButtonsComponent;
  let fixture: ComponentFixture<AuthorizationConsentButtonsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorizationConsentButtonsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorizationConsentButtonsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
