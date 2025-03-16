import { Component } from '@angular/core';
import { CoinsListComponent } from "../coins-list/coins-list.component";
import { CoinNewsListComponent } from "../coin-news-list/coin-news-list.component";

@Component({
  selector: 'app-dash-board',
  imports: [CoinsListComponent, CoinNewsListComponent],
  templateUrl: './dash-board.component.html',
  styleUrl: './dash-board.component.scss'
})
export class DashBoardComponent {

}
