import {Component, inject, OnInit} from '@angular/core';
import {BaseListPagedComponent} from '../../../core/components/base-classes/base-list-paged-component';
import {ListEventsQueryDto, ListEventsRequest} from '../../../api-services/events/events-api.model';
import {EventsApiService} from '../../../api-services/events/events-api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../core/services/toaster.service';
import {DialogHelperService} from '../../shared/services/dialog-helper.service';
import { DialogButton } from '../../shared/models/dialog-config.model';

@Component({
  selector: 'app-events',
  standalone: false,
  templateUrl: './events.component.html',
  styleUrl: './events.component.scss',
  providers: [EventsApiService]
})
export class EventsComponent
  extends BaseListPagedComponent<ListEventsQueryDto, ListEventsRequest>
  implements OnInit{
  private api = inject(EventsApiService);
  private dialogHelper = inject(DialogHelperService);

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
}
