import {Component, inject, OnInit} from '@angular/core';
import {BaseListPagedComponent} from '../../../core/components/base-classes/base-list-paged-component';
import {ListEventsQueryDto, ListEventsRequest} from '../../../api-services/events/events-api.model';
import {EventsApiService} from '../../../api-services/events/events-api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../core/services/toaster.service';
import {DialogHelperService} from '../../shared/services/dialog-helper.service';

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
  private router = inject(Router);
  private toaster = inject(ToasterService);
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

  onCreate(): void {
    this.router.navigate(['organizer/events/add']);
  }

  onEdit(event: ListEventsQueryDto): void {
    this.router.navigate(['organizer/events', event.id, 'edit']);
  }

  onDelete(event: number): void {}

  onSearchChange(searchTerm: string) : void {
    this.request.search = searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }
}
