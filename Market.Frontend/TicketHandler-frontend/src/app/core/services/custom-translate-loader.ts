import {TranslateLoader} from '@ngx-translate/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import { inject } from '@angular/core';

export class CustomTranslateLoader implements TranslateLoader {
  constructor(private http: HttpClient) {
   
  }
  getTranslation(lang: string): Observable<any> {
    return this.http.get(`/i18n/${lang}.json`); // Folder za prevode
  }
}
