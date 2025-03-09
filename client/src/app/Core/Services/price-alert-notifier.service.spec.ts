import { TestBed } from '@angular/core/testing';

import { PriceAlertNotifierService } from './price-alert-notifier.service';

describe('PriceAlertNotifierService', () => {
  let service: PriceAlertNotifierService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PriceAlertNotifierService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
