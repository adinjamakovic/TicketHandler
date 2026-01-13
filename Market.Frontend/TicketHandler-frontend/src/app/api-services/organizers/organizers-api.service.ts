import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { CreateOrganizerCommand, GetOrganizerByIdQueryDto, ListOrganizersRequest, ListOrganizersResponse, UpdateOrganizerCommand } from "./organizers-api.model";
import { Observable } from "rxjs";
import { buildHttpParams } from "../../core/models/build-http-params";

@Injectable({
    providedIn: 'root'
})
export class OrganizerApiService {
    private readonly baseUrl=`${environment.apiUrl}/Organizers`;
    private http = inject(HttpClient);

    //GET /Organizers
    //List organizers
    list(request?: ListOrganizersRequest) : Observable<ListOrganizersResponse>{
        const params = request ? buildHttpParams(request as any) : undefined;

        return this.http.get<ListOrganizersResponse>(this.baseUrl, {
            params,
        });
    }

    //Get /Organizer by Id
    getById(id: number): Observable<GetOrganizerByIdQueryDto> {
        return this.http.get<GetOrganizerByIdQueryDto>(`${this.baseUrl}/${id}`);
    }

    //POST /Organizers
    create(payload: CreateOrganizerCommand): Observable<number>{
        return this.http.post<number>(this.baseUrl, payload);
    }

    //PUT /Organizers/{id}
    update(id: number, payload: UpdateOrganizerCommand) : Observable<void>{
        return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
    }

    //DELETE /Organizers/{id}
    delete(id: number) : Observable<void>{
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}