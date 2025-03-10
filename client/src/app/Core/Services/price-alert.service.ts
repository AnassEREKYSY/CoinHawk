import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../envirnoments/envirnoments.development';
import { CoinDto } from '../Dtos/CoinDto';
import { PriceAlertDto } from '../Dtos/PriceAlertDto';

@Injectable({
  providedIn: 'root'
})
export class PriceAlertService {

  constructor(private http: HttpClient) {}

  createPriceAlert(body:PriceAlertDto): Observable<PriceAlertDto[]> {
    const token = localStorage.getItem('auth_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.post<PriceAlertDto[]>(`${environment.apiUrl}price-alerts/create-alert`,body ,{ headers });
  }


  deletePriceAlertById(id:string): Observable<any> {
    const token = localStorage.getItem('auth_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<any>(`${environment.apiUrl}price-alerts/delete-alert/`+id,{ headers });
  }

  deletePriceAlertByCoinId(coinId: string): Observable<any> {
    const token = localStorage.getItem('auth_token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
    return this.http.delete<any>(
      `${environment.apiUrl}price-alerts/delete-alerts-by-coin/` + coinId,
      { headers, responseType: 'text' as 'json' }
    );
  }
  
}
