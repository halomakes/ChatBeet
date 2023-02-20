import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrewmatesComponent } from './crewmates.component';

describe('CrewmatesComponent', () => {
  let component: CrewmatesComponent;
  let fixture: ComponentFixture<CrewmatesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CrewmatesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CrewmatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
