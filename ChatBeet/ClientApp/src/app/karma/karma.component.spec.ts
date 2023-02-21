import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KarmaComponent } from './karma.component';

describe('KarmaComponent', () => {
  let component: KarmaComponent;
  let fixture: ComponentFixture<KarmaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KarmaComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(KarmaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
