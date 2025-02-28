import { Component } from '@angular/core';
import { NavBarComponent } from "../../Shared/nav-bar/nav-bar.component";
import { CoinsListComponent } from "../coins-list/coins-list.component";

@Component({
  selector: 'app-dash-board',
  imports: [NavBarComponent, CoinsListComponent],
  templateUrl: './dash-board.component.html',
  styleUrl: './dash-board.component.scss'
})
export class DashBoardComponent {

}
