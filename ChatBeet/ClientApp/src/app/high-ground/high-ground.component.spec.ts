import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HighGroundComponent } from './high-ground.component';

describe('HighGroundComponent', () => {
  let component: HighGroundComponent;
  let fixture: ComponentFixture<HighGroundComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HighGroundComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HighGroundComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
