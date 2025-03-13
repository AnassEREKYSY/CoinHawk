import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../envirnoments/envirnoments.development';
import { CoinDto } from '../Dtos/CoinDto';
import { PriceAlertDto } from '../Dtos/PriceAlertDto';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class PriceAlertService {

  constructor(private http: HttpClient,  private keycloakService: KeycloakService) {}

  async createPriceAlert(body:PriceAlertDto): Promise<Observable<PriceAlertDto[]>> {
    const token = await this.keycloakService.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<PriceAlertDto[]>(`${environment.apiUrl}price-alerts/create-alert`,body ,{ headers });
  }


  async deletePriceAlertById(id:string): Promise<Observable<any>> {
    const token = await this.keycloakService.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<any>(`${environment.apiUrl}price-alerts/delete-alert/`+id,{ headers });
  }

  async deletePriceAlertByCoinId(coinId: string): Promise<Observable<any>> {
    const token = await this.keycloakService.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<any>(
      `${environment.apiUrl}price-alerts/delete-alerts-by-coin/` + coinId,
      { headers, responseType: 'text' as 'json' }
    );
  }
  
}
