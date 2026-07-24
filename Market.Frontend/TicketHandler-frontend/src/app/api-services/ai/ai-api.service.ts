import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GenerateChatReplyCommand, GenerateChatReplyResponse } from './ai-api.model';

@Injectable({
  providedIn: 'root',
})
export class AiApiService {
  private readonly baseUrl = `${environment.apiUrl}/Ai`;
  private http = inject(HttpClient);

  chat(payload: GenerateChatReplyCommand): Observable<GenerateChatReplyResponse> {
    return this.http.post<GenerateChatReplyResponse>(`${this.baseUrl}/chat`, payload);
  }
}
