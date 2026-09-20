import {Component, inject, OnInit} from '@angular/core';
import {BaseListPagedComponent} from '../../../core/components/base-classes/base-list-paged-component';
import {ListEventsQueryDto, ListEventsRequest} from '../../../api-services/events/events-api.model';
import {EventsApiService} from '../../../api-services/events/events-api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../core/services/toaster.service';
import {DialogHelperService} from '../../shared/services/dialog-helper.service';
import { DialogButton } from '../../shared/models/dialog-config.model';
import { EventTypesApiService } from '../../../api-services/event-types/event-types-api.service';
import { ListEventTypesQueryDto, ListEventTypesRequest } from '../../../api-services/event-types/event-types-api.models';
import { toDateOnlyParam } from '../../../core/utils/date-filter-utils';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { DdMmYyyyDateAdapter, DD_MM_YYYY_FORMATS } from '../../../core/utils/DateUtilities/datepicker-utils';

@Component({
  selector: 'app-events',
  standalone: false,
  templateUrl: './events.component.html',
  styleUrl: './events.component.scss',
  providers: [
    EventsApiService,
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS }
  ]
})
export class EventsComponent
  extends BaseListPagedComponent<ListEventsQueryDto, ListEventsRequest>
  implements OnInit{
  private api = inject(EventsApiService);
  private eventTypesApi = inject(EventTypesApiService);
  private dialogHelper = inject(DialogHelperService);

  cities: string[] = [];
  eventTypes: ListEventTypesQueryDto[] = [];

  displayedColumns: string[] = [
    'name',
    'description',
    'scheduledDate',
    'venueName',
    'organizer',
    'eventType',
    'actions'
  ];

  constructor() {
    super();
    this.request = new ListEventsRequest();
    this.request.paging.pageSize = 5
  }
  ngOnInit(): void {
        this.loadFilterOptions();
        this.initList();
  }

  private loadFilterOptions(): void {
    this.api.getCities().subscribe({
      next: (cities) => this.cities = cities,
      error: (err) => console.error('Load event cities error', err)
    });

    const eventTypesRequest = new ListEventTypesRequest();
    eventTypesRequest.paging.pageSize = 100;

    this.eventTypesApi.list(eventTypesRequest).subscribe({
      next: (response) => this.eventTypes = response.items,
      error: (err) => console.error('Load event types error', err)
    });
  }

  protected override loadPagedData(): void {
    this.startLoading();
    this.api.list(this.buildQuery()).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load events');
        console.error('Load events error', err);
      }
    });
  }

  onDelete(event: ListEventsQueryDto): void {
    this.dialogHelper.confirmDelete(event.name).subscribe(result => {
          if (result && result.button === DialogButton.DELETE) {
            this.performDelete(event);
          }
        });
  }

  private performDelete(event: ListEventsQueryDto): void {
      this.startLoading();
  
      this.api.delete(event.id).subscribe({
        next: () => {
          this.dialogHelper.showSuccess("Succesfully deleted event", `You have succesfully deleted ${event.name}`).subscribe();
          this.loadPagedData();
        },
        error: (err) => {
          this.stopLoading();
  
          this.dialogHelper.showError(
            'Error while deleting',
            `Unable to delete ${event.name}`
          ).subscribe();
  
          console.error('Delete event error:', err);
        }
      });
    }

  onSearchChange(searchTerm: string) : void {
    this.request.search = searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  /** Sends the picked calendar days instead of their UTC instants. */
  private buildQuery(): ListEventsRequest {
    return {
      ...this.request,
      dateFrom: toDateOnlyParam(this.request.dateFrom),
      dateTo: toDateOnlyParam(this.request.dateTo)
    };
  }

  /** Re-runs the query from page one — used by every filter control. */
  onFilterChange(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onClearFilters(): void {
    this.request.search = null;
    this.request.eventType = null;
    this.request.city = null;
    this.request.dateFrom = null;
    this.request.dateTo = null;
    this.onFilterChange();
  }

  get hasActiveFilters(): boolean {
    return !!(this.request.search
      || this.request.eventType
      || this.request.city
      || this.request.dateFrom
      || this.request.dateTo);
  }
}
