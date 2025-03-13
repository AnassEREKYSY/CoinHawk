import { Component, OnInit } from '@angular/core';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { CoinService } from '../../Core/Services/coin.service';
import { Router } from '@angular/router';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { CoinComponent } from '../coin/coin.component';
import { CommonModule } from '@angular/common';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { PriceAlertNotifierService } from '../../Core/Services/price-alert-notifier.service';
import { Subscription } from 'rxjs';
import { PriceAlertService } from '../../Core/Services/price-alert.service';

@Component({
  selector: 'app-coins-list',
  templateUrl: './coins-list.component.html',
  styleUrls: ['./coins-list.component.scss'],
  imports: [
    CoinComponent,
    CommonModule,
    InfiniteScrollModule
  ],
  standalone: true
})
export class CoinsListComponent implements OnInit {
  private allCoins: CoinDto[] = [];

  coinsData: CoinDto[] = [];

  isLoading = true;  
  isLoadingNewData = false;

  scrollDistance = 2;
  scrollUpDistance = 1.5;

  private pageSize = 10;
  private displayedCount = 0;
  private priceAlertSub!: Subscription;

  constructor(
    private snackBarService: SnackBarService,
    private coinService: CoinService,
    private priceAlertNotifierService: PriceAlertNotifierService,
    private priceAlertService: PriceAlertService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAllFollowedCoins();
    this.priceAlertSub = this.priceAlertNotifierService.priceAlert$.subscribe(() => {
      this.loadAllFollowedCoins();
    });
  }

  async loadAllFollowedCoins() {
    this.isLoading = true;
    (await this.coinService.getFollowedCoins()).subscribe({
      next: (coins) => {
        this.allCoins = coins || [];
        this.coinsData = [];
        this.displayedCount = 0;
        
        this.isLoading = false;

        this.loadMoreItems();
      },
      error: (error) => {
        this.snackBarService.error(error);
        this.isLoading = false;
      }
    });
  }

  loadMore() {
    if (this.displayedCount >= this.allCoins.length) {
      return;
    }

    this.isLoadingNewData = true;
    setTimeout(() => {
      this.loadMoreItems();
      this.isLoadingNewData = false;
    }, 500);
  }

  private loadMoreItems() {
    const nextItems = this.allCoins.slice(this.displayedCount, this.displayedCount + this.pageSize);
    this.coinsData.push(...nextItems);
    this.displayedCount += nextItems.length;
  }

  async unfollowCoin(coin: CoinDto): Promise<void> {
    (await this.priceAlertService.deletePriceAlertByCoinId(coin.id)).subscribe({
      next: () => {
        this.snackBarService.success("Coin unfollowed successfully");
        this.loadAllFollowedCoins();
      },
      error: (error) => {
        this.snackBarService.error(error);
      }
    });
  }
}
