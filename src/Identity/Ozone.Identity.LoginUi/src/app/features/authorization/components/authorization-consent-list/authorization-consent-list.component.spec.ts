import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationConsentListComponent } from './authorization-consent-list.component';

describe('AuthorizationConsentListComponent', () => {
  let component: AuthorizationConsentListComponent;
  let fixture: ComponentFixture<AuthorizationConsentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorizationConsentListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorizationConsentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
