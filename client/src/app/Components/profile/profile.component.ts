import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Core/Services/user.service';
import { Router } from '@angular/router';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { ProfileDto } from '../../Core/Dtos/ProfileDto';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-profile',
  imports: [
    MatCardModule,
    CommonModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

  userProfile: ProfileDto | undefined;
  errorMessage: string | undefined;

  constructor(
    private userService: UserService,
    private snackBarService: SnackBarService,
    private router: Router
  ){}


  ngOnInit(): void {
    this.getUserProfile();
  }

  getUserProfile(){
    this.userService.getUserProfile().subscribe(
      (response) => {
        console.log(response)
        this.userProfile=response;
      },
      () => {
        this.snackBarService.error("Failed to get user's profile ")
      }
    );
  }


}
