import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import {
  CreateEventNewsCommand,
  GetEventNewsByIdQueryDto, ListEventNewsRequest, ListEventNewsResponse, UpdateEventNewsCommand
} from "./event-news-api.model";
import { Observable } from "rxjs";
import { buildHttpParams } from "../../core/models/build-http-params";
import { FormDataUtils } from "../../core/utils/FormUtilities/form-data-utils";

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
    GetById(id: number): Observable<GetEventNewsByIdQueryDto> {
        return this.http.get<GetEventNewsByIdQueryDto>(`${this.baseUrl}/${id}`);
    }

    //POST /EventNews
    create(payload: CreateEventNewsCommand): Observable<Number> {
        const formData = FormDataUtils.toFormData(payload);
        return this.http.post<number>(this.baseUrl, formData);
    }

    //PUT /EventNews/{id}
    update(id: number, payload: UpdateEventNewsCommand): Observable<void>{
        const formData = FormDataUtils.toFormData(payload);
        return this.http.put<void>(`${this.baseUrl}/${id}`, formData);
    }

    //DELETE /EventNews/{id}
    delete(id: number): Observable<void>{
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
