import { Component, OnInit } from '@angular/core';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { CoinService } from '../../Core/Services/coin.service';
import { Router } from '@angular/router';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { CoinComponent } from "../coin/coin.component";
import { CommonModule } from '@angular/common';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
@Component({
  selector: 'app-coins-list',
  imports: [
    CoinComponent,
    CommonModule,
    InfiniteScrollModule,
  ],
  templateUrl: './coins-list.component.html',
  styleUrl: './coins-list.component.scss'
})
export class CoinsListComponent implements OnInit {
  coinsData: CoinDto[] = [];
  isLoading = true;
  isLoadingNewData = false;
  scrollDistance = 2;
  scrollUpDistance = 1.5;
  coinsStaticData: CoinDto[] = [
    {
      id: 'bitcoin',
      name: 'Bitcoin',
      symbol: 'BTC',
      currentPrice: 57324.67,
      marketCap: 1123456789123,
      totalVolume: 456789123,
      thumb: 'https://assets.coingecko.com/coins/images/1/thumb/bitcoin.png',
      large: 'https://assets.coingecko.com/coins/images/1/large/bitcoin.png',
      marketChart: [
        [1708785600, 57000],
        [1708789200, 57200],
        [1708792800, 57350],
      ],
    },
    {
      id: 'ethereum',
      name: 'Ethereum',
      symbol: 'ETH',
      currentPrice: 3245.12,
      marketCap: 345678912345,
      totalVolume: 123456789,
      thumb: 'https://assets.coingecko.com/coins/images/279/thumb/ethereum.png',
      large: 'https://assets.coingecko.com/coins/images/279/large/ethereum.png',
      marketChart: [
        [1708785600, 3200],
        [1708789200, 3225],
        [1708792800, 3245],
      ],
    },
    {
      id: 'binancecoin',
      name: 'Binance Coin',
      symbol: 'BNB',
      currentPrice: 412.89,
      marketCap: 98765432123,
      totalVolume: 56789123,
      thumb: 'https://assets.coingecko.com/coins/images/825/thumb/binance-coin.png',
      large: 'https://assets.coingecko.com/coins/images/825/large/binance-coin.png',
      marketChart: [
        [1708785600, 400],
        [1708789200, 410],
        [1708792800, 412],
      ],
    },
    {
      id: 'ripple',
      name: 'XRP',
      symbol: 'XRP',
      currentPrice: 1.23,
      marketCap: 56789123456,
      totalVolume: 34567891,
      thumb: 'https://assets.coingecko.com/coins/images/44/thumb/xrp-symbol-white-128.png',
      large: 'https://assets.coingecko.com/coins/images/44/large/xrp-symbol-white-128.png',
      marketChart: [
        [1708785600, 1.20],
        [1708789200, 1.22],
        [1708792800, 1.23],
      ],
    },
    {
      id: 'ripple',
      name: 'XRP',
      symbol: 'XRP',
      currentPrice: 1.23,
      marketCap: 56789123456,
      totalVolume: 34567891,
      thumb: 'https://assets.coingecko.com/coins/images/44/thumb/xrp-symbol-white-128.png',
      large: 'https://assets.coingecko.com/coins/images/44/large/xrp-symbol-white-128.png',
      marketChart: [
        [1708785600, 1.20],
        [1708789200, 1.22],
        [1708792800, 1.23],
      ],
    }
  ];
  

  constructor(
    private snackBarService: SnackBarService,
    private coinService: CoinService,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.coinsData=this.coinsStaticData
    // this.getFollowedCoins();
  }

  getFollowedCoins() {
    this.coinService.getFollowedCoins().subscribe({
      next: (coins) => {
        this.coinsData = coins;
        this.isLoading = false;
      },
      error: (error) => {
        this.snackBarService.error(error);
        this.isLoading = false;
      }
    });
  }

  loadMore() {
    this.isLoadingNewData = true;
  }

  unfollowCoin(coinId: string): void {

  }
  
}
