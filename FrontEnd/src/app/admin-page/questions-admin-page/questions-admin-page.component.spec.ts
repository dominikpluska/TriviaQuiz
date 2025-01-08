import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuestionsAdminPageComponent } from './questions-admin-page.component';

describe('QuestionsAdminPageComponent', () => {
  let component: QuestionsAdminPageComponent;
  let fixture: ComponentFixture<QuestionsAdminPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuestionsAdminPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuestionsAdminPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
