import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscordContentComponent } from './discord-content.component';

describe('DiscordContentComponent', () => {
  let component: DiscordContentComponent;
  let fixture: ComponentFixture<DiscordContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DiscordContentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DiscordContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
