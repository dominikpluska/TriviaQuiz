import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionDetailsAdminPageComponent } from './question-details-admin-page.component';

describe('QuestionDetailsAdminPageComponent', () => {
  let component: QuestionDetailsAdminPageComponent;
  let fixture: ComponentFixture<QuestionDetailsAdminPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionDetailsAdminPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuestionDetailsAdminPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
