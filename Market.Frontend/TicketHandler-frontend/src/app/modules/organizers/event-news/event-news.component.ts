import {Component, inject, OnInit} from '@angular/core';
import {BaseListPagedComponent} from '../../../core/components/base-classes/base-list-paged-component';
import {ListEventNewsQueryDto, ListEventNewsRequest} from '../../../api-services/event-news/event-news-api.model';
import {EventNewsApiService} from '../../../api-services/event-news/event-news-api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../core/services/toaster.service';
import {DialogHelperService} from '../../shared/services/dialog-helper.service';
import {MatDialog} from '@angular/material/dialog';
import {EventNewsUpsertComponent} from './event-news-upsert/event-news-upsert.component';
import {ListEventsQueryDto} from '../../../api-services/events/events-api.model';
import {DialogButton} from '../../shared/models/dialog-config.model';

@Component({
  selector: 'app-event-news',
  standalone: false,
  templateUrl: './event-news.component.html',
  styleUrl: './event-news.component.scss',
})
export class EventNewsComponent
  extends BaseListPagedComponent<ListEventNewsQueryDto, ListEventNewsRequest>
  implements OnInit {
    private api = inject(EventNewsApiService);
    private router = inject(Router);
    private toaster = inject(ToasterService);
    private dialogHelper = inject(DialogHelperService);
    private dialog = inject(MatDialog);
    displayedColumns: string[] = [
      'event',
      'header',
      'body',
      'actions'
    ];

    constructor() {
      super();
      this.request = new ListEventNewsRequest();
    }

    ngOnInit(): void {
      this.initList();
    }

    protected override loadPagedData(): void {
        this.startLoading();

      this.api.list(this.request).subscribe({
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

  onCreate(): void {
      const dialogRef = this.dialog.open(EventNewsUpsertComponent, {
        width: '500px',
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
        width: '500px',
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
