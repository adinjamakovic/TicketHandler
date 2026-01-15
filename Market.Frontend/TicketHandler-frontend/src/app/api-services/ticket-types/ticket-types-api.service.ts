import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { ListOrganizersRequest, UpdateOrganizerCommand } from '../organizers/organizers-api.model';
import { Observable } from 'rxjs';
import { CreateTicketTypesCommand, GetTicketTypesByIdQueryDto, ListTicketTypesRequest, ListTicketTypesResponse, UpdateTicketTypesCommand } from './ticket-types-api.model';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class TicketTypesApiService {
  private readonly baseUrl = `${environment.apiUrl}/TicketType`
  private http = inject(HttpClient);
    //GET /TicketTypes
    //List TicketTypes
  list(request?: ListTicketTypesRequest) : Observable<ListTicketTypesResponse>{
    const params = request ? buildHttpParams(request as any) : undefined;
    
    return this.http.get<ListTicketTypesResponse>(this.baseUrl,{
      params,
    });
  }
  //Get /TicketTypes by Id
  getById(id:number ): Observable<GetTicketTypesByIdQueryDto>{
    return this.http.get<GetTicketTypesByIdQueryDto>(`${this.baseUrl}/${id}`);
  }
  //POST /TicketTypes
  create(payload: CreateTicketTypesCommand): Observable<number>{
    return this.http.post<number>(this.baseUrl,payload);
  }
  //PUT /TicketTypes/{id}
  update(id:number,payload:UpdateTicketTypesCommand) : Observable<void>{
    return this.http.put<void>(`${this.baseUrl}/${id}`,payload);
  }
  //DELETE /TicketTypes/{id}
  delete(id:number): Observable<void>{
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
