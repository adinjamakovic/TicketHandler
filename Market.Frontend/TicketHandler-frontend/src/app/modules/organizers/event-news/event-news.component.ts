import {Component, inject, OnInit} from '@angular/core';
import {BaseListPagedComponent} from '../../../core/components/base-classes/base-list-paged-component';
import {ListEventNewsQueryDto, ListEventNewsRequest} from '../../../api-services/event-news/event-news-api.model';
import {EventNewsApiService} from '../../../api-services/event-news/event-news-api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../core/services/toaster.service';
import {DialogHelperService} from '../../shared/services/dialog-helper.service';
import {MatDialog} from '@angular/material/dialog';
import {EventNewsUpsertComponent} from './event-news-upsert/event-news-upsert.component';
import {ListEventsQueryDto, ListEventsRequest} from '../../../api-services/events/events-api.model';
import {DialogButton} from '../../shared/models/dialog-config.model';
import { EventsApiService } from '../../../api-services/events/events-api.service';
import { toDateOnlyParam } from '../../../core/utils/date-filter-utils';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { DdMmYyyyDateAdapter, DD_MM_YYYY_FORMATS } from '../../../core/utils/DateUtilities/datepicker-utils';

@Component({
  selector: 'app-event-news',
  standalone: false,
  templateUrl: './event-news.component.html',
  styleUrl: './event-news.component.scss',
  providers: [
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS }
  ],
})
export class EventNewsComponent
  extends BaseListPagedComponent<ListEventNewsQueryDto, ListEventNewsRequest>
  implements OnInit {
    private api = inject(EventNewsApiService);
    private eventsApi = inject(EventsApiService);
    private toaster = inject(ToasterService);
    private dialogHelper = inject(DialogHelperService);
    private dialog = inject(MatDialog);
    events: ListEventsQueryDto[] = [];
    displayedColumns: string[] = [
      'event',
      'header',
      'body',
      'actions'
    ];

    constructor() {
      super();
      this.request = new ListEventNewsRequest();
      this.request.paging.pageSize = 5;
      this.loadEvents();
    }

    private loadEvents() : void {
        // The dropdown needs every event the organiser owns, not just the first default page.
        const eventsRequest = new ListEventsRequest();
        eventsRequest.paging.pageSize = 100;

        this.eventsApi.list(eventsRequest).subscribe({
          next: (response)=>{
            this.events = response.items;
          },
          error: (err) => {
            this.toaster.error('Failed to load events.');
            console.error('Load events error', err);
          }
        });
    }

    ngOnInit(): void {
      this.initList();
    }

    protected override loadPagedData(): void {
        this.startLoading();

      this.api.list(this.buildQuery()).subscribe({
        next: (response) => {
          this.handlePageResult(response);
          this.stopLoading();
        },
        error: (err) => {
          this.stopLoading('Failed to load event news');
          console.error('Event news error:', err);
        }
      });
    }

  onSearchChange(eventId: number): void {
    this.request.eventId = eventId;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  /** Sends the picked calendar days instead of their UTC instants. */
  private buildQuery(): ListEventNewsRequest {
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
    this.request.eventId = null;
    this.request.search = null;
    this.request.dateFrom = null;
    this.request.dateTo = null;
    this.request.hasImage = null;
    this.onFilterChange();
  }

  get hasActiveFilters(): boolean {
    return !!(this.request.eventId
      || this.request.search
      || this.request.dateFrom
      || this.request.dateTo
      // "Without image" is `false`, which is a filter just like `true` is.
      || (this.request.hasImage !== null && this.request.hasImage !== undefined));
  }

  onCreate(): void {
      const dialogRef = this.dialog.open(EventNewsUpsertComponent, {
        width: '715px',
        maxWidth: '90vw',
        panelClass: 'event-news-dialog',
        autoFocus: true,
        disableClose: false,
        data: {
          mode: 'create',
        },
      });

      dialogRef.afterClosed().subscribe((success: boolean) => {
        if (success) {
          this.dialogHelper.eventNews.showCreateSuccess().subscribe();
          this.loadPagedData();
        }
      });
  }

  onEdit(news: ListEventNewsQueryDto): void {
      const dialogRef = this.dialog.open(EventNewsUpsertComponent, {
        width: '715px',
        maxWidth: '90vw',
        panelClass: 'event-news-dialog',
        autoFocus: true,
        disableClose: false,
        data: {
          mode: 'edit',
          eventNewsId: news.id,
        },
      });

      dialogRef.afterClosed().subscribe((success: boolean) => {
        if (success) {
          this.dialogHelper.eventNews.showUpdateSuccess().subscribe();
          this.loadPagedData();
        }
      });
  }

  onDelete(eventNews: ListEventNewsQueryDto): void {
      this.dialogHelper.eventNews.confirmDelete(eventNews.header).subscribe(result => {
        if (result && result.button === DialogButton.DELETE) {
          this.performDelete(eventNews);
        }
      });
  }

  private performDelete(eventNews: ListEventNewsQueryDto) : void {
        this.startLoading();

        this.api.delete(eventNews.id).subscribe({
          next: () => {
            this.dialogHelper.eventNews.showDeleteSuccess().subscribe();
            this.loadPagedData();
          },
          error: (err) => {
            this.stopLoading();

            const errorMessage = this.extractErrorMessage(err);

            this.dialogHelper.showError(
              'ERROR!',
              'Failed to delete event news.',
            ).subscribe();
            console.error('Delete event news error', err);

            },
        });
    }

  private extractErrorMessage(err: any): string | null {
    if (err?.error) {
      if (typeof err.error === 'string') {
        return err.error;
      }

      if (err.error.message) {
        return err.error.message;
      }

      if (err.error.title) {
        return err.error.title;
      }

      if (err.error.errors && typeof err.error.errors === 'object') {
        const errors = Object.values(err.error.errors).flat();
        if (errors.length > 0) {
          return errors.join(', ');
        }
      }
    }

    if (err?.message) {
      return err.message;
    }

    if (err?.statusText && err.statusText !== 'Unknown Error') {
      return err.statusText;
    }

    return null;
  }
}
