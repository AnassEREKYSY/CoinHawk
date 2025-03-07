import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { CoinService } from '../../Core/Services/coin.service';
import { CoinDto } from '../../Core/Dtos/CoinDto';

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

  constructor(
    private router: Router,
    private coinService: CoinService
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
}
