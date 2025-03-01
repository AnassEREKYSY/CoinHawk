import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Core/Services/user.service';
import { Router } from '@angular/router';
import { SnackBarService } from '../../Core/Services/snack-bar.service';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
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
      },
      () => {
        this.snackBarService.error("Failed to get user's profile ")
      }
    );
  }


}
