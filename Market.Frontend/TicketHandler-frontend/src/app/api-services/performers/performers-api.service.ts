import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../environments/environment';
import {GetPerformerByIdQueryDto, ListPerformersRequest, ListPerformersResponse} from './performers-api.models';
import {buildHttpParams} from '../../core/models/build-http-params';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class PerformersApiService {
  private readonly baseUrl: string = `${environment.apiUrl}/performers`;
  private http = inject(HttpClient);

  //GET /Performers
  list(request?: ListPerformersRequest): Observable<ListPerformersResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;

    return this.http.get<ListPerformersResponse>(this.baseUrl, {
      params,
    });
  }

  //GET /Performers/{id}
  getById(id: number): Observable<GetPerformerByIdQueryDto> {
    return this.http.get<GetPerformerByIdQueryDto>(`${this.baseUrl}/${id}`);
  }
}
