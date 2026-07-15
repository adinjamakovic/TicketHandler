import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { EventsApiService } from '../../../../api-services/events/events-api.service';
import {
  DdMmYyyyDateAdapter,
  DD_MM_YYYY_FORMATS,
} from '../../../../core/utils/DateUtilities/datepicker-utils';

export interface LandingSearchEvent {
  search?: string | null;
  dateFrom?: Date | null;
  dateTo?: Date | null;
  city?: string | null;
}

@Component({
  selector: 'app-landing-page-search',
  standalone: false,
  templateUrl: './landing-page-search.component.html',
  styleUrl: './landing-page-search.component.scss',
  providers: [
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS },
  ],
})
export class LandingPageSearchComponent implements OnInit {
  @Output() search = new EventEmitter<LandingSearchEvent>();

  form!: FormGroup;
  cities: string[] = [];

  private fb = inject(FormBuilder);
  private eventsApi = inject(EventsApiService);

  ngOnInit(): void {
    this.form = this.fb.group({
      search: [null],
      dateFrom: [null],
      dateTo: [null],
      city: [null],
    });

    this.eventsApi.getCities().subscribe(cities => (this.cities = cities));
  }

  onSearch(): void {
    this.search.emit(this.form.value as LandingSearchEvent);
  }

  onClear(): void {
    this.form.reset();
    this.search.emit({});
  }
}
