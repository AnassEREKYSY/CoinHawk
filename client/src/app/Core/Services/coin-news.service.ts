import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import { NewsArticleDto } from '../Dtos/NewsArticleDto';
import { Observable } from 'rxjs';
import { environment } from '../../../envirnoments/envirnoments.development';

@Injectable({
  providedIn: 'root'
})
export class CoinNewsService {

  constructor(private http: HttpClient,  private keycloakService: KeycloakService) {}
  
    async getFollowedCoinsNews(): Promise<Observable<NewsArticleDto[]>> {
      const token = await this.keycloakService.getToken();
      const headers = new HttpHeaders({
        Authorization: `Bearer ${token}`,
      });
      return this.http.get<NewsArticleDto[]>(`${environment.apiUrl}price-alerts/coin-news`,{ headers });
    }
}
