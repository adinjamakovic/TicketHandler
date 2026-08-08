import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ConfirmPaymentCommand,
  ConfirmPaymentCommandDto,
  CreatePaymentIntentCommand,
  CreatePaymentIntentCommandDto,
  GetPaymentConfigQueryDto,
  GetPaymentQuoteQueryDto,
} from './payments-api.models';

@Injectable({
  providedIn: 'root'
})
export class PaymentsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Payments`;
  private http = inject(HttpClient);

  config(): Observable<GetPaymentConfigQueryDto> {
    return this.http.get<GetPaymentConfigQueryDto>(`${this.baseUrl}/config`);
  }

  quote(): Observable<GetPaymentQuoteQueryDto> {
    return this.http.get<GetPaymentQuoteQueryDto>(`${this.baseUrl}/quote`);
  }

  createIntent(payload: CreatePaymentIntentCommand): Observable<CreatePaymentIntentCommandDto> {
    return this.http.post<CreatePaymentIntentCommandDto>(`${this.baseUrl}/intent`, payload);
  }

  confirm(payload: ConfirmPaymentCommand): Observable<ConfirmPaymentCommandDto> {
    return this.http.post<ConfirmPaymentCommandDto>(`${this.baseUrl}/confirm`, payload);
  }
}
