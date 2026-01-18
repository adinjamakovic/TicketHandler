import {inject, Injectable} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {ListVenuesRequest, ListVenuesResponse} from './venues-api.models';
import {Observable} from 'rxjs';
import {buildHttpParams} from '../../core/models/build-http-params';

@Injectable({
  providedIn: 'root'
})
export class VenuesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Venues`;
  private http = inject(HttpClient);


  //Get /Venues
  list(request?: ListVenuesRequest) : Observable<ListVenuesResponse>
  {
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListVenuesResponse>(this.baseUrl, {
      params
    });
  }
}
