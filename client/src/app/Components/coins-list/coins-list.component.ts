import { Component, OnInit } from '@angular/core';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { CoinService } from '../../Core/Services/coin.service';
import { Router } from '@angular/router';
import { CoinDto } from '../../Core/Dtos/CoinDto';

@Component({
  selector: 'app-coins-list',
  imports: [],
  templateUrl: './coins-list.component.html',
  styleUrl: './coins-list.component.scss'
})
export class CoinsListComponent implements OnInit {
  coinsData:CoinDto[]=[];

  constructor(
      private snackBarService: SnackBarService,
      private coinService: CoinService,
      private router: Router
  ) {}

  ngOnInit(): void {
    
  }

  getFollowedCoins(){
    
  }
}
