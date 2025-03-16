import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CoinNewsListComponent } from './coin-news-list.component';

describe('CoinNewsListComponent', () => {
  let component: CoinNewsListComponent;
  let fixture: ComponentFixture<CoinNewsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CoinNewsListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CoinNewsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
