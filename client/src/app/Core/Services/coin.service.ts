import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../envirnoments/envirnoments.development';
import { CoinDto } from '../Dtos/CoinDto';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class CoinService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private keycloakService: KeycloakService) {}

  getAllCoins(): Observable<CoinDto[]> {
    return this.http.get<CoinDto[]>(`${this.apiUrl}coins/get-all-coins`);
  }

  getCoinData(coinName: string): Observable<CoinDto> {
    return this.http.get<CoinDto>(`${this.apiUrl}coins/get-coin-data/${coinName}`);
  }

  getCoinMarketChart(coinName: string, days: number): Observable<number[][]> {
    return this.http.get<number[][]>(`${this.apiUrl}coins/get-coin-market-chart/${coinName}/market-chart?days=${days}`);
  }

  getCoinOhlc(coinName: string, days: number): Observable<number[][]> {
    return this.http.get<number[][]>(`${this.apiUrl}coins/get-coin-ohlc/${coinName}?days=${days}`);
  }  

  getCoinInfo(coinName: string, days: number): Observable<CoinDto> {
    return this.http.get<CoinDto>(`${this.apiUrl}coins/get-coin-details-info/${coinName}?days=${days}`);
  }

  getTrendingCoins(): Observable<CoinDto[]> {
    return this.http.get<CoinDto[]>(`${this.apiUrl}coins/get-trending-coins`);
  }

  async getFollowedCoins(): Promise<Observable<CoinDto[]>> {
    const token = await this.keycloakService.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.get<CoinDto[]>(`${environment.apiUrl}price-alerts/get-followed-coins`, { headers });
  }

  searchCoins(query: string): Observable<CoinDto[]> {
    return this.http.get<CoinDto[]>(`${this.apiUrl}coins/search-coin?query=${query}`);
  }
  
}
