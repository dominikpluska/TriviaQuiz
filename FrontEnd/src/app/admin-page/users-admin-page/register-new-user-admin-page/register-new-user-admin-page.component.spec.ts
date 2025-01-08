import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterNewUserAdminPageComponent } from './register-new-user-admin-page.component';

describe('RegisterNewUserAdminPageComponent', () => {
  let component: RegisterNewUserAdminPageComponent;
  let fixture: ComponentFixture<RegisterNewUserAdminPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterNewUserAdminPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterNewUserAdminPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
