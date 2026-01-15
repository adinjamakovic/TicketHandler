import { Component, inject } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { EventsApiService } from '../../../api-services/events/events-api.service';
import { PersonApiService } from '../../../api-services/person/person.api.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { GetPersonByIdQueryDto } from '../../../api-services/person/person.api.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-organizers-layout',
  standalone: false,
  templateUrl: './organizers-layout.component.html',
  styleUrl: './organizers-layout.component.scss',
})
export class OrganizersLayoutComponent {
   auth = inject(AuthFacadeService)
  private personService = inject(PersonApiService);
  private translate = inject(TranslateService);

  currentLang: string;

  languages = [
    { code: 'bs', name: 'Bosanski', flag: '🇧🇦' },
    { code: 'en', name: 'English', flag: '🇬🇧' }
  ];

  constructor() {
    this.currentLang = this.translate.currentLang || 'bs';
  }

  switchLanguage(langCode: string): void {
    this.currentLang = langCode;
    this.translate.use(langCode);
    localStorage.setItem('language', langCode);
  }

  getCurrentLanguage() {
    return this.languages.find(lang => lang.code === this.currentLang);
  }
}

