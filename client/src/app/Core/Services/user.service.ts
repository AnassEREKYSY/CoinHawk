import { Injectable } from '@angular/core';
import { environment } from '../../../envirnoments/envirnoments.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProfileDto } from '../Dtos/ProfileDto';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getUserProfile(): Observable<ProfileDto[]> {
    const token = localStorage.getItem('auth_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<ProfileDto[]>(`${environment.apiUrl}users/profile`, { headers });
  }
}
