import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../Core/Services/auth.service'; 
import { Router } from '@angular/router';
import { SnackBarService } from '../../Core/Services/snack-bar.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule,],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBarService: SnackBarService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]], 
      password: ['', [Validators.required, Validators.minLength(6)]], 
    });
  }

  login(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const loginRequest = this.loginForm.value;

    this.authService.login(loginRequest).subscribe(
      (response) => {
        this.authService.setToken(response.token);
        this.router.navigate(['/dashboard']); 
        this.snackBarService.success("You're logged successfully")
      },
      () => {
        this.snackBarService.error('Login failed try again ')
      }
    );
  }

  RedirectToRegister(){
    this.router.navigate(['/register']); 
  }
  
}
