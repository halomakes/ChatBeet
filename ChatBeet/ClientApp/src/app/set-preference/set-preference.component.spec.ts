import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetPreferenceComponent } from './set-preference.component';

describe('SetPreferenceComponent', () => {
  let component: SetPreferenceComponent;
  let fixture: ComponentFixture<SetPreferenceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SetPreferenceComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SetPreferenceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
