import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PriceAlertNotifierService {
  private priceAlertSubject = new Subject<void>();
  priceAlert$ = this.priceAlertSubject.asObservable();

  notifyPriceAlert(): void {
    this.priceAlertSubject.next();
  }
}
