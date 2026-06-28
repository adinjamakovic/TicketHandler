import {inject, Injectable} from '@angular/core';
import {environment} from '../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {ListEventTypesRequest, ListEventTypesResponse} from './event-types-api.models';
import {Observable} from 'rxjs';
import {buildHttpParams} from '../../core/models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class EventTypesApiService {
  private readonly baseUrl = `${environment.apiUrl}/EventTypes`;
  private http = inject(HttpClient);

  //GET /EventTypes
  list(request?: ListEventTypesRequest): Observable<ListEventTypesResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListEventTypesResponse>(this.baseUrl, {
      params,
    });
  }
}
