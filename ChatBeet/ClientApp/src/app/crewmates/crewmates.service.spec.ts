import { TestBed } from '@angular/core/testing';

import { CrewmatesService } from './crewmates.service';

describe('CrewmatesService', () => {
  let service: CrewmatesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CrewmatesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
