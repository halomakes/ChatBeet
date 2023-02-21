import { TestBed } from '@angular/core/testing';

import { HighGroundService } from './high-ground.service';

describe('HighGroundService', () => {
  let service: HighGroundService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HighGroundService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
