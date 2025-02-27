import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-bar',
  imports: [
    MatIconModule,
    FormsModule,
    CommonModule
  ],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss'
})
export class NavBarComponent implements OnInit {
  searchQuery: string = '';
  searchResults: any[] = [];
  selectedFilter: string = 'all'; 
  loading: boolean = false;
  offset: number = 0;
  limit: number = 10;

  constructor(
    private router: Router,
  ) {}

  ngOnInit(): void {

  }
   
   
  navigateToHome(): void {
    this.router.navigate(['/dahsboard']);
  }

  navigateToProfile(): void {
    this.router.navigate(['/profile']);
  }

  logout(){
    localStorage.removeItem('auth_token');
    this.router.navigate(['/login']);
  }

  onSearchInput(): void {
    
  }
  
  

  onScroll(): void {
    
  }

  loadSearchResults(): void {
   
  }
}
