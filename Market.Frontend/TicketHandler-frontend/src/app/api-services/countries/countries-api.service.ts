import {inject, Injectable} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {ListCountriesRequest, ListCountriesResponse} from './countries-api.models';
import {Observable} from 'rxjs';
import {buildHttpParams} from '../../core/models/build-http-params';

@Injectable({
  providedIn: 'root'
})
export class CountriesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Countries`;
  private http = inject(HttpClient);

  list(request?: ListCountriesRequest): Observable<ListCountriesResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListCountriesResponse>(this.baseUrl, {
      params,
    });
  }
}
