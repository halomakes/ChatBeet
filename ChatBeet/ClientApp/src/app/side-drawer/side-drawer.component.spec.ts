import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SideDrawerComponent } from './side-drawer.component';

describe('SideDrawerComponent', () => {
  let component: SideDrawerComponent;
  let fixture: ComponentFixture<SideDrawerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SideDrawerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SideDrawerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
