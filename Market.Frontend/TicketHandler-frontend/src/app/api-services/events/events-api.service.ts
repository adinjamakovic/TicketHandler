import {inject, Injectable} from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import {
  CreateEventCommand,
  GetEventByIdQueryDto,
  GetEventByOrganizerIdResponse,
  GetEventsByOrganizerIdRequest,
  ListEventsQueryDto,
  ListEventsRequest,
  ListEventsResponse,
  ListEventsWithPerformersRequest,
  ListEventsWithPerformersResponse,
  UpdateEventCommand
} from "./events-api.model";
import { Observable } from "rxjs";
import { buildHttpParams } from "../../core/models/build-http-params";

@Injectable({
  providedIn: 'root',
})
export class EventsApiService{
    private readonly baseUrl = `${environment.apiUrl}/Events`;
    private http = inject(HttpClient);

    //GET /Events
    list(request?: ListEventsRequest) : Observable<ListEventsResponse>{
        const params = request ? buildHttpParams(request as any) : undefined;

        return this.http.get<ListEventsResponse>(this.baseUrl, {
            params,
        });
    }

    //GET /Events/with-performers
    listWithPerformers(request?: ListEventsWithPerformersRequest) : Observable<ListEventsWithPerformersResponse>{
        const params = request ? buildHttpParams(request as any) : undefined;

        return this.http.get<ListEventsWithPerformersResponse>(`${this.baseUrl}/with-performers`, {
            params,
        });
    }

    //GET /Events/OrganizerId
    getByOrganizerId(request?: GetEventsByOrganizerIdRequest) : Observable<GetEventByOrganizerIdResponse>{
        const params = request ? buildHttpParams(request as any) : undefined;

        return this.http.get<GetEventByOrganizerIdResponse>(`${this.baseUrl}/OrganizerId`, {
            params,
        });
    }

    //GET /Events/{id}
    getById(id: number): Observable<GetEventByIdQueryDto> {
        return this.http.get<GetEventByIdQueryDto>(`${this.baseUrl}/${id}`);
    }

    //POST /Events
    create(payload: CreateEventCommand) : Observable<number>{
        return this.http.post<number>(this.baseUrl, payload);
    }

    //PUT /Events
    update(id: number, payload: UpdateEventCommand) : Observable<void>{
        return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
    }

    //DELETE /Events/{id}
    delete(id: number): Observable<void>{
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
