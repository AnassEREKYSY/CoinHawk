import { Injectable } from '@angular/core';
import { environment } from '../../../envirnoments/envirnoments.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProfileDto } from '../Dtos/ProfileDto';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient, private keycloakService: KeycloakService) {}

  async getUserProfile(): Promise<Observable<ProfileDto>> {
    const token = await this.keycloakService.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<ProfileDto>(`${environment.apiUrl}users/profile`, { headers });
  }
}
