import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventsApiService } from '../../../api-services/events/events-api.service';
import { GetEventByIdQueryDto, ListEventsQueryDto } from '../../../api-services/events/events-api.model';
import { TicketsApiService } from '../../../api-services/tickets/tickets-api.service';
import { ListTicketsQueryDto } from '../../../api-services/tickets/tickets-api.model';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-event-details',
  standalone: false,
  templateUrl: './event-details.component.html',
  styleUrl: './event-details.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventDetailsComponent implements OnInit {
  event: GetEventByIdQueryDto | null = null;
  tickets: ListTicketsQueryDto[] = [];
  relatedEvents: ListEventsQueryDto[] = [];

  isLoading = true;
  ticketsLoading = true;
  relatedLoading = true;
  notFound = false;
  showQrDialog = false;

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventsApi = inject(EventsApiService);
  private ticketsApi = inject(TicketsApiService);
  private toaster = inject(ToasterService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      this.loadEvent(id);
    });
  }

  copyLink(): void {
    navigator.clipboard.writeText(window.location.href).then(
      () => this.toaster.success('Event link copied to clipboard'),
      () => this.toaster.error('Could not copy the link')
    );
  }

  openQrDialog(): void {
    this.showQrDialog = true;
  }

  closeQrDialog(): void {
    this.showQrDialog = false;
  }

  onRelatedEventSelected(event: ListEventsQueryDto): void {
    this.router.navigate(['/event', event.id]);
    window.scrollTo({ top: 0 });
  }

  goToAllEvents(): void {
    this.router.navigate(['/']);
  }

  private loadEvent(id: number): void {
    this.isLoading = true;
    this.notFound = false;
    this.event = null;
    this.tickets = [];
    this.relatedEvents = [];
    this.showQrDialog = false;
    this.cdr.markForCheck();

    this.eventsApi.getById(id).subscribe({
      next: event => {
        this.event = event;
        this.isLoading = false;
        this.loadTickets(event);
        this.loadRelated(event);
        this.cdr.markForCheck();
      },
      error: () => {
        this.isLoading = false;
        this.notFound = true;
        this.cdr.markForCheck();
      },
    });
  }

  private loadRelated(event: GetEventByIdQueryDto): void {
    this.relatedLoading = true;
    this.cdr.markForCheck();

    this.eventsApi
      .list({ paging: { page: 1, pageSize: 100 } } as any)
      .subscribe({
        next: result => {
          this.relatedEvents = (result.items ?? []).filter(
            e => e.id !== event.id && e.eventType === event.eventTypeName
          );
          this.relatedLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.relatedEvents = [];
          this.relatedLoading = false;
          this.cdr.markForCheck();
        },
      });
  }

  private loadTickets(event: GetEventByIdQueryDto): void {
    this.ticketsLoading = true;
    this.cdr.markForCheck();

    this.ticketsApi
      .list({ eventName: event.name, paging: { page: 1, pageSize: 50 } } as any)
      .subscribe({
        next: result => {
          this.tickets = (result.items ?? []).filter(t => t.event.id === event.id);
          this.ticketsLoading = false;
          this.cdr.markForCheck();
        },
        error: () => {
          this.tickets = [];
          this.ticketsLoading = false;
          this.cdr.markForCheck();
        },
      });
  }
}
