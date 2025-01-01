import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinishGamePageComponent } from './finish-game-page.component';

describe('FinishGamePageComponent', () => {
  let component: FinishGamePageComponent;
  let fixture: ComponentFixture<FinishGamePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FinishGamePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FinishGamePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
