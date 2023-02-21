import { ComponentFixture, TestBed } from '@angular/core/testing';

import ProgressPreviewComponent from './progress-preview.component';

describe('ProgressPreviewComponent', () => {
  let component: ProgressPreviewComponent;
  let fixture: ComponentFixture<ProgressPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProgressPreviewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProgressPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
