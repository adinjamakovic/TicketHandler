import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CreateTicketsCommand, GetTicketsByEventIdQueryDto, GetTicketsByEventNameQueryDto, GetTicketsByIdQueryDto, ListTicketsRequest, ListTicketsResponse, UpdateTicketsCommand } from './tickets-api.model';
import { Observable } from 'rxjs';
import { buildHttpParams } from '../../core/models/build-http-params';

@Injectable({
  providedIn: 'root',
})
export class TicketsApiService {
  private readonly baseUrl= `${environment.apiUrl}/Tickets`;
  private http = inject(HttpClient);

    //GET /Tickets
      //List Tickets
    list(request?: ListTicketsRequest) : Observable<ListTicketsResponse>{
      const params = request ? buildHttpParams(request as any) : undefined;
      return this.http.get<ListTicketsResponse>(this.baseUrl,{
        params,
      });
    }
    //Get /Tickets by Id
    getById(id:number ): Observable<GetTicketsByIdQueryDto>{
      return this.http.get<GetTicketsByIdQueryDto>(`${this.baseUrl}/${id}`);
    }
     //Get /Tickets by EventId
    getByEventId(id:number ): Observable<GetTicketsByEventIdQueryDto>{
      return this.http.get<GetTicketsByEventIdQueryDto>(`${this.baseUrl}/EventId`);
    }
     //Get /Tickets by EventName
    getByEventName(eventName: string): Observable<GetTicketsByEventNameQueryDto>{
      return this.http.get<GetTicketsByEventNameQueryDto>(`${this.baseUrl}/EventName/${eventName}`);
    }
    //POST /Tickets
    create(payload: CreateTicketsCommand): Observable<number>{
      return this.http.post<number>(this.baseUrl,payload);
    }
    //PUT /Tickets/{id}
    update(id:number,payload:UpdateTicketsCommand) : Observable<void>{
      return this.http.put<void>(`${this.baseUrl}/${id}`,payload);
    }
    //DELETE /Tickets/{id}
    delete(id:number): Observable<void>{
      return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
