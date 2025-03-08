import { Component } from '@angular/core';
import { CoinsListComponent } from "../coins-list/coins-list.component";

@Component({
  selector: 'app-dash-board',
  imports: [CoinsListComponent],
  templateUrl: './dash-board.component.html',
  styleUrl: './dash-board.component.scss'
})
export class DashBoardComponent {

}
