import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { provideNativeDateAdapter } from '@angular/material/core';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { EventsApiService } from '../../../api-services/events/events-api.service';
import { ListEventsQueryDto, ListEventsRequest } from '../../../api-services/events/events-api.model';
import { EventTypesApiService } from '../../../api-services/event-types/event-types-api.service';
import { ListEventTypesQueryDto } from '../../../api-services/event-types/event-types-api.models';
import { LandingSearchEvent } from '../../shared/components/landing-page-search/landing-page-search.component';

@Component({
  selector: 'app-list-events',
  standalone: false,
  templateUrl: './list-events.component.html',
  styleUrl: './list-events.component.scss',
  providers: [provideNativeDateAdapter()],
})
export class ListEventsComponent
  extends BaseListPagedComponent<ListEventsQueryDto, ListEventsRequest>
  implements OnInit {

  eventTypes: ListEventTypesQueryDto[] = [];
  selectedEventType: string | null = null;
  initialFilters: LandingSearchEvent = {};

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventsApi = inject(EventsApiService);
  private eventTypesApi = inject(EventTypesApiService);

  ngOnInit(): void {
    this.request = new ListEventsRequest();
    this.request.paging.pageSize = 15;

    this.eventTypesApi.list().subscribe(result => (this.eventTypes = result.items ?? []));

    this.route.queryParamMap.subscribe(params => {
      const dateFrom = params.get('dateFrom');
      const dateTo = params.get('dateTo');

      this.request.search = params.get('search');
      this.request.city = params.get('city');
      this.request.dateFrom = dateFrom ? new Date(dateFrom) : null;
      this.request.dateTo = dateTo ? new Date(dateTo) : null;
      this.request.eventType = params.get('eventType');

      this.selectedEventType = this.request.eventType;
      this.initialFilters = {
        search: this.request.search,
        dateFrom: this.request.dateFrom,
        dateTo: this.request.dateTo,
        city: this.request.city,
      };

      this.request.paging.page = 1;
      this.loadPagedData();
    });
  }

  onSearch(filters: LandingSearchEvent): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        search: filters.search || null,
        city: filters.city || null,
        dateFrom: filters.dateFrom ? filters.dateFrom.toISOString() : null,
        dateTo: filters.dateTo ? filters.dateTo.toISOString() : null,
      },
      queryParamsHandling: 'merge',
    });
  }

  selectEventType(eventType: string | null): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { eventType },
      queryParamsHandling: 'merge',
    });
  }

  openEvent(event: ListEventsQueryDto): void {
    this.router.navigate(['/event', event.id]);
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.eventsApi.list(this.request).subscribe({
      next: result => {
        this.handlePageResult(result);
        this.stopLoading();
      },
      error: () => {
        this.items = [];
        this.totalItems = 0;
        this.totalPages = 0;
        this.stopLoading('Failed to load events.');
      },
    });
  }
}
