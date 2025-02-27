import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../Core/Services/auth.service'; 
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup | undefined;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]], // Email validation
      password: ['', [Validators.required, Validators.minLength(6)]], // Password validation
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
        this.router.navigate(['/profile']); // Redirect to profile
      },
      (error) => {
        this.errorMessage = 'Login failed: ' + error.message;
      }
    );
  }
  
}
