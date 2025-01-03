import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatsDetailsPageComponent } from './stats-details-page.component';

describe('StatsDetailsPageComponent', () => {
  let component: StatsDetailsPageComponent;
  let fixture: ComponentFixture<StatsDetailsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatsDetailsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StatsDetailsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
