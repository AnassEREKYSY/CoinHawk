import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CoinDto } from '../../Core/Dtos/CoinDto';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';

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
  @Output() unfollow: EventEmitter<string> = new EventEmitter<string>();

  unfollowCoin(event: Event): void {
    event.stopPropagation();
    this.unfollow.emit(this.coin.id);
  }

  goToCoinDetails(arg0: string) {
    throw new Error('Method not implemented.');
  }
}
