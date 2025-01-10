import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterNewQuestionAdminPageComponent } from './register-new-question-admin-page.component';

describe('RegisterNewQuestionAdminPageComponent', () => {
  let component: RegisterNewQuestionAdminPageComponent;
  let fixture: ComponentFixture<RegisterNewQuestionAdminPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterNewQuestionAdminPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterNewQuestionAdminPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
