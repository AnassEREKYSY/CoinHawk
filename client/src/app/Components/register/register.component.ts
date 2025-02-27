import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../Core/Services/auth.service';
import { SnackBarService } from '../../Core/Services/snack-bar.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  RegisterForm!: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBarService: SnackBarService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.RegisterForm = this.fb.group({
      lastName: ['', [Validators.required,]], 
      firstName: ['', [Validators.required,]], 
      email: ['', [Validators.required, Validators.email]], 
      password: ['', [Validators.required, Validators.minLength(6)]], 
    });
  }

  register(): void {
    if (this.RegisterForm.invalid) {
      return;
    }

    const registerRequest = this.RegisterForm.value;

    this.authService.register(registerRequest).subscribe(
      () => {
        this.router.navigate(['/login']); 
        this.snackBarService.success("You're registered successfully")
      },
      () => {
        this.snackBarService.error('Register failed try again ')
      }
    );
  }
}
