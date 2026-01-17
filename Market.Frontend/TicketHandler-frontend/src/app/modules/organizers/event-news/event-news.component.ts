import {Component, inject, OnInit} from '@angular/core';
import {BaseListPagedComponent} from '../../../core/components/base-classes/base-list-paged-component';
import {ListEventNewsQueryDto, ListEventNewsRequest} from '../../../api-services/event-news/event-news-api.model';
import {EventNewsApiService} from '../../../api-services/event-news/event-news-api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../core/services/toaster.service';
import {DialogHelperService} from '../../shared/services/dialog-helper.service';
import {MatDialog} from '@angular/material/dialog';
import {EventNewsUpsertComponent} from './event-news-upsert/event-news-upsert.component';

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
        panelClass: 'product-category-dialog',
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

  onEdit(eventId: number): void {}

  onDelete(eventId: number): void {}

}
