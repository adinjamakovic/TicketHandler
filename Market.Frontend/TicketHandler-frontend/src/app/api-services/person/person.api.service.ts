import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GetPersonByIdQueryDto } from './person.api.model';

@Injectable({
  providedIn: 'root',
})
export class PersonApiService {
  private readonly baseUrl = `${environment.apiUrl}/Person`;
  private http = inject(HttpClient);
  /**
     * GET /Person/{id}
     * Get a single Person by ID.
     */
    getById(id: number): Observable<GetPersonByIdQueryDto> {
      return this.http.get<GetPersonByIdQueryDto>(`${this.baseUrl}/${id}`);
    }
}
