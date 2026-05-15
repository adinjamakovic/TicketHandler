import {inject, Injectable} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {ListCitiesRequest, ListCitiesResponse} from './cities-api.models';
import {Observable} from 'rxjs';
import {buildHttpParams} from '../../core/models/build-http-params';

@Injectable({
  providedIn: 'root'
})
export class CitiesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Cities`;
  private http = inject(HttpClient);

  list(request?: ListCitiesRequest): Observable<ListCitiesResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListCitiesResponse>(this.baseUrl, {
      params,
    });
  }
}
