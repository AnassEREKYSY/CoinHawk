import { Injectable } from '@angular/core';
import { environment } from '../../../envirnoments/envirnoments.development';
import { HttpClient } from '@angular/common/http';
import { LoginDto } from '../Dtos/LoginDto';
import { Observable } from 'rxjs';
import { RegisterDto } from '../Dtos/RegisterDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  login(request: LoginDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}users/login`, request);
  }

  register(request: RegisterDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}users/register`, request);
  }

  setToken(token: string): void {
    localStorage.setItem('auth_token', token); 
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }
  isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null;
  }

  logout(): void {
    localStorage.removeItem('auth_token');
  }
}
