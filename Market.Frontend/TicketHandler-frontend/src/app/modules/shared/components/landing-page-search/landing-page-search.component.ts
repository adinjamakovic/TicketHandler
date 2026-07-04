import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { EventsApiService } from '../../../../api-services/events/events-api.service';

export interface LandingSearchEvent {
  search?: string | null;
  date?: Date | null;
  city?: string | null;
}

@Component({
  selector: 'app-landing-page-search',
  standalone: false,
  templateUrl: './landing-page-search.component.html',
  styleUrl: './landing-page-search.component.scss',
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
      date: [null],
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
