import { TestBed } from '@angular/core/testing';

import { CoinNewsService } from './coin-news.service';

describe('CoinNewsService', () => {
  let service: CoinNewsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CoinNewsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
