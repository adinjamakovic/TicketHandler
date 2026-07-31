import { inject, Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { GetDashboardQueryDto } from "./dashboard-api.model";

@Injectable({
    providedIn: 'root'
})
export class DashboardApiService {
    private readonly baseUrl = `${environment.apiUrl}/Dashboard`;
    private http = inject(HttpClient);

    get(): Observable<GetDashboardQueryDto> {
        return this.http.get<GetDashboardQueryDto>(this.baseUrl);
    }
}
