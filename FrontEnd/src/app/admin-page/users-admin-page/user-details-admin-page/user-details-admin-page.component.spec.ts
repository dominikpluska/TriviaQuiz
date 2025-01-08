import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserDetailsAdminPageComponent } from './user-details-admin-page.component';

describe('UserDetailsAdminPageComponent', () => {
  let component: UserDetailsAdminPageComponent;
  let fixture: ComponentFixture<UserDetailsAdminPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserDetailsAdminPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserDetailsAdminPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
