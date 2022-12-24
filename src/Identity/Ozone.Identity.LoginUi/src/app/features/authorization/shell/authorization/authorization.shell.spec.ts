import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorizationShell } from './authorization.shell';

describe('AuthorizationShell', () => {
  let component: AuthorizationShell;
  let fixture: ComponentFixture<AuthorizationShell>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AuthorizationShell ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuthorizationShell);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
