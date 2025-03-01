import { Routes } from '@angular/router';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { DashBoardComponent } from './Components/dash-board/dash-board.component';
import { NavBarComponent } from './Shared/nav-bar/nav-bar.component';
import { CoinComponent } from './Components/coin/coin.component';
import { TrendingCoinsComponent } from './Components/trending-coins/trending-coins.component';
import { ProfileComponent } from './Components/profile/profile.component';
import { CoinsListComponent } from './Components/coins-list/coins-list.component';
import { CoinDetailsComponent } from './Components/coin-details/coin-details.component';

export const routes: Routes = [
    { path: '', component: LoginComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'dashboard', component: DashBoardComponent },
    { path: 'nav-bar', component: NavBarComponent },
    { path: 'coin', component: CoinComponent },
    { path: 'coin-list', component: CoinsListComponent },
    { path: 'coin-details/:coinName', component: CoinDetailsComponent },
    { path: 'trending-coins', component: TrendingCoinsComponent },
    { path: 'profile', component: ProfileComponent },
];
