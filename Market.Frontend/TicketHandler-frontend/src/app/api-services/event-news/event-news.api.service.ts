import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { CreateEventNewsCommand, ListEventNewsRequest, ListEventNewsResponse, UpdateEventNewsCommand } from "./event-news-api.model";
import { Observable } from "rxjs";
import { buildHttpParams } from "../../core/models/build-http-params";
import { GetEventByIdQueryDto } from "../events/events-api.model";

@Injectable({
    providedIn: 'root'
})
export class EventNewsApiService {
    private readonly baseUrl = `${environment.apiUrl}/EventNews`;
    private http = inject(HttpClient);

    //GET /EventNews
    list(request?: ListEventNewsRequest): Observable<ListEventNewsResponse>{
        const params = request ? buildHttpParams(request as any): undefined;

        return this.http.get<ListEventNewsResponse>(this.baseUrl, {
            params,
        });
    }

    //GET /EventNews/{id}
    GetById(id: number): Observable<GetEventByIdQueryDto> {
        return this.http.get<GetEventByIdQueryDto>(`${this.baseUrl}/${id}}`);
    }

    //POST /EventNews
    create(payload: CreateEventNewsCommand): Observable<Number> {
        return this.http.post<number>(this.baseUrl, payload);
    }

    //PUT /EventNews/{id}
    update(id: number, payload: UpdateEventNewsCommand): Observable<void>{
        return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
    }

    //DELETE /EventNews/{id}
    delete(id: number): Observable<void>{
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}