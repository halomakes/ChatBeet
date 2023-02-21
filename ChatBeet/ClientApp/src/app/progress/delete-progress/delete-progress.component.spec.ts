import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteProgressComponent } from './delete-progress.component';

describe('DeleteProgressComponent', () => {
  let component: DeleteProgressComponent;
  let fixture: ComponentFixture<DeleteProgressComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeleteProgressComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteProgressComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
