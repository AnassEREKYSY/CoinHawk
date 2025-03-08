import { TestBed } from '@angular/core/testing';

import { PriceAlertService } from './price-alert.service';

describe('PriceAlertService', () => {
  let service: PriceAlertService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PriceAlertService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
