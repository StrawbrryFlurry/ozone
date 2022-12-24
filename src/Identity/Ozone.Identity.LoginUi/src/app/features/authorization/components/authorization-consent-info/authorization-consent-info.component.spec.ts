import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationConsentInfoComponent } from './authorization-consent-info.component';

describe('AuthorizationConsentInfoComponent', () => {
  let component: AuthorizationConsentInfoComponent;
  let fixture: ComponentFixture<AuthorizationConsentInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorizationConsentInfoComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorizationConsentInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
