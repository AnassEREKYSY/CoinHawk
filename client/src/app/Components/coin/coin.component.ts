import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-coin',
  imports: [
    MatIconModule,
    CommonModule
  ],
  templateUrl: './coin.component.html',
  styleUrl: './coin.component.scss'
})
export class CoinComponent {
  @Input() coin!: CoinDto;
  @Output() unfollow: EventEmitter<CoinDto> = new EventEmitter<CoinDto>();

  constructor(private router:Router){}

  unfollowCoin(event: Event): void {
    event.stopPropagation();
    this.unfollow.emit(this.coin);
  }

  goToDetails(name: string) {
    this.router.navigate(['/coin-details', name]); 
  }
}
