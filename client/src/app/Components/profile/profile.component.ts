import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Core/Services/user.service';
import { Router } from '@angular/router';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { ProfileDto } from '../../Core/Dtos/ProfileDto';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    MatCardModule,
    CommonModule,
    MatButtonModule,
    MatIconModule,
    ReactiveFormsModule
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

  userProfile: ProfileDto | undefined;
  errorMessage: string | undefined;
  isEditing: boolean = false;
  profileForm: FormGroup;
  selectedFile: File | null = null;

  constructor(
    private userService: UserService,
    private snackBarService: SnackBarService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.profileForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    this.getUserProfile();
  }

  getUserProfile(){
    this.userService.getUserProfile().subscribe(
      (response) => {
        this.userProfile = response;
        this.profileForm.patchValue({
          email: response.email,
          password: '' // Leave password empty for security reasons
        });
      },
      () => {
        this.snackBarService.error("Failed to get user's profile");
      }
    );
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  submitForm() {
    // if (this.profileForm.valid) {
    //   const updatedData = new FormData();
    //   updatedData.append('email', this.profileForm.value.email);
    //   updatedData.append('password', this.profileForm.value.password);

    //   if (this.selectedFile) {
    //     updatedData.append('image', this.selectedFile);
    //   }

    //   this.userService.updateUserProfile(updatedData).subscribe(
    //     () => {
    //       this.snackBarService.success("Profile updated successfully!");
    //       this.isEditing = false;
    //       this.getUserProfile();
    //     },
    //     () => {
    //       this.snackBarService.error("Failed to update profile.");
    //     }
    //   );
    // }
  }
}
