import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationConsentItemComponent } from './authorization-consent-item.component';

describe('AuthorizationConsentItemComponent', () => {
  let component: AuthorizationConsentItemComponent;
  let fixture: ComponentFixture<AuthorizationConsentItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorizationConsentItemComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorizationConsentItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
