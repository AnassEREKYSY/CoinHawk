import { CommonModule } from '@angular/common';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { CoinService } from '../../Core/Services/coin.service';
import { PriceAlertService } from '../../Core/Services/price-alert.service';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { PriceAlertNotifierService } from '../../Core/Services/price-alert-notifier.service';

@Component({
  selector: 'app-nav-bar',
  imports: [
    MatIconModule,
    FormsModule,
    CommonModule,
    MatButtonModule,
  ],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent implements OnInit {
  searchQuery: string = '';
  searchResults: CoinDto[] = [];
  activeFollowCoin: CoinDto | null = null;
  followTargetPrices: { [key: string]: number } = {};
  followFormPosition: { top: number, left: number } | null = null;

  constructor(
    private router: Router,
    private coinService: CoinService,
    private priceAlertService: PriceAlertService,
    private snackBarService: SnackBarService,
    private priceAlertNotifierService: PriceAlertNotifierService,
  ) {}

  ngOnInit(): void {}

  navigateToHome(): void {
    this.router.navigate(['/dashboard']);
  }

  navigateToProfile(): void {
    this.router.navigate(['/profile']);
  }

  logout(){
    localStorage.removeItem('auth_token');
    this.router.navigate(['/login']);
  }

  onSearchInput(): void {
    if (!this.searchQuery.trim()) {
      this.searchResults = [];
      return;
    }

    this.coinService.searchCoins(this.searchQuery)
      .subscribe({
        next: (results) => {
          this.searchResults = results;
        },
        error: (error) => {
          console.error('Search error:', error);
          this.searchResults = [];
        }
      });
  }

  toggleFollowForm(coin: CoinDto, event: MouseEvent): void {
    if (this.activeFollowCoin && this.activeFollowCoin.id === coin.id) {
      this.activeFollowCoin = null;
      this.followFormPosition = null;
    } else {
      const rect = (event.target as HTMLElement).getBoundingClientRect();
      this.followFormPosition = { top: rect.top, left: rect.right + 8 };
      this.activeFollowCoin = coin;
    }
  }

  submitFollow(): void {
    if (!this.activeFollowCoin) return;
    const coinName = this.activeFollowCoin.name;
    const targetPrice = this.followTargetPrices[this.activeFollowCoin.id];
    const body = { coinName, targetPrice };
    this.priceAlertService.createPriceAlert(body).subscribe({
      next: () => {
        this.snackBarService.success('Price Alert Created');
        this.activeFollowCoin = null;
        this.followFormPosition = null;
        this.priceAlertNotifierService.notifyPriceAlert();
      },
      error: (error) => {
        this.snackBarService.error('Error creating price alert: ' + error);
      }
    });
  }
}
