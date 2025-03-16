import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoinNewsComponent } from './coin-news.component';

describe('CoinNewsComponent', () => {
  let component: CoinNewsComponent;
  let fixture: ComponentFixture<CoinNewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoinNewsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoinNewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
