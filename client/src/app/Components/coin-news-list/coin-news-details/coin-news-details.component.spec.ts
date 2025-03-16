import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoinNewsDetailsComponent } from './coin-news-details.component';

describe('CoinNewsDetailsComponent', () => {
  let component: CoinNewsDetailsComponent;
  let fixture: ComponentFixture<CoinNewsDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoinNewsDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoinNewsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
