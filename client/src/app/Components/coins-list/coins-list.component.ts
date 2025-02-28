import { Component, OnInit } from '@angular/core';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { CoinService } from '../../Core/Services/coin.service';
import { Router } from '@angular/router';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { CoinComponent } from '../coin/coin.component';
import { CommonModule } from '@angular/common';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

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
  // Holds all followed coins from the server
  private allCoins: CoinDto[] = [];

  // Coins currently displayed in the template
  coinsData: CoinDto[] = [];

  // Control flags
  isLoading = true;  
  isLoadingNewData = false;

  // InfiniteScroll distance settings
  scrollDistance = 2;
  scrollUpDistance = 1.5;

  // Show data in chunks of 10
  private pageSize = 10;
  private displayedCount = 0;

  constructor(
    private snackBarService: SnackBarService,
    private coinService: CoinService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAllFollowedCoins();
  }

  // Fetch all the user's coins once
  private loadAllFollowedCoins() {
    this.isLoading = true;
    this.coinService.getFollowedCoins().subscribe({
      next: (coins) => {
        this.allCoins = coins || [];
        this.isLoading = false;

        // Show initial 10 items
        this.loadMoreItems();
      },
      error: (error) => {
        this.snackBarService.error(error);
        this.isLoading = false;
      }
    });
  }

  // Triggered by infinite-scroll when user scrolls near bottom
  loadMore() {
    if (this.displayedCount >= this.allCoins.length) {
      return; // no more items
    }

    this.isLoadingNewData = true;
    // Optionally add a short delay if you want a spinner effect
    setTimeout(() => {
      this.loadMoreItems();
      this.isLoadingNewData = false;
    }, 500);
  }

  // Appends the next chunk of 10 items from allCoins to coinsData
  private loadMoreItems() {
    const nextItems = this.allCoins.slice(this.displayedCount, this.displayedCount + this.pageSize);
    this.coinsData.push(...nextItems);
    this.displayedCount += nextItems.length;
  }

  // Example unfollow logic if needed
  unfollowCoin(coinId: string): void {
    // this.coinsData = this.coinsData.filter(c => c.id !== coinId);
    // this.allCoins = this.allCoins.filter(c => c.id !== coinId);
    // this.displayedCount = this.coinsData.length;
  }
}
