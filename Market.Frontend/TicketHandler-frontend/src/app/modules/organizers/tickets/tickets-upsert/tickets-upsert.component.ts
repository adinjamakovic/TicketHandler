import { BaseListComponent } from '../../../../core/components/base-classes/base-list-component';
import { CreateTicketsCommand, ListTicketsQueryDto, ListTicketsRequest, UpdateTicketsCommand } from '../../../../api-services/tickets/tickets-api.model';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { TicketsApiService } from '../../../../api-services/tickets/tickets-api.service';
import { TicketsFormService } from '../services/tickets-form.service';
import { EventsApiService } from '../../../../api-services/events/events-api.service';
import { Observable } from 'rxjs/internal/Observable';
import { startWith } from 'rxjs/internal/operators/startWith';
import { map } from 'rxjs/internal/operators/map';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToasterService } from '../../../../core/services/toaster.service';
import { Component, inject } from '@angular/core';
import { GetEventsByOrganizerIdDto, GetEventsByOrganizerIdRequest, ListEventsQueryDto } from '../../../../api-services/events/events-api.model';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';
import { TicketTypesApiService } from '../../../../api-services/ticket-types/ticket-types-api.service';
import { GetTicketTypesByIdQueryDto, ListTicketTypesQueryDto } from '../../../../api-services/ticket-types/ticket-types-api.model';
import { tap } from 'rxjs';
import { setValue } from '@ngx-translate/core';
import { T } from '@angular/cdk/keycodes';
import { OrganizerApiService } from '../../../../api-services/organizers/organizers-api.service';

export interface TicketsDialogData {
  mode: 'create' | 'edit';
  ticketId?: number;
}

@Component({
  selector: 'app-tickets-upsert',
  standalone: false,
  templateUrl: './tickets-upsert.component.html',
  styleUrl: './tickets-upsert.component.scss',
  providers: [TicketsFormService,EventsApiService,GetEventsByOrganizerIdRequest,OrganizerApiService],
})
export class TicketsUpsertComponent extends BaseListPagedComponent<ListTicketsQueryDto,ListTicketsRequest>{
  private api = inject(TicketsApiService);
  private data = inject<TicketsDialogData>(MAT_DIALOG_DATA);
  private toaster = inject(ToasterService);
  private dialogRef = inject(MatDialogRef<TicketsUpsertComponent>);
  private eventsApi = inject(EventsApiService);
  private formService = inject(TicketsFormService);
  private ticketTypesApi = inject(TicketTypesApiService);
  private currentUserService = inject(CurrentUserService);
  private eventsByOrganizerRequest = inject(GetEventsByOrganizerIdRequest);
  private organizersApiService = inject(OrganizerApiService);
  
  title = '';
  form!:FormGroup;
  eventID!:number;
  benefits!:string;
  unitPrice!:number;
  isEditMode = false;
  ticketTypeID!:number;
  events: string[] = [];
  ticketTypes:string[]=[];
  quantityInStock!:number;
  filteredEvents: Observable<string[]> | undefined;
  filteredTicketTypes: Observable<string[]> | undefined;
  mappedEvents!: Map<number, string>;
  mappedTicketTypes!: Map<number, string>;
  override isLoading = false;
  currentOrgId:number = 1;
  currentUserId:number | undefined;

    // TODO NAPRAVIT SERVIS ZA VRACANJE IDA OD ORG PA ONDA VIDIT ZASTO FILTER NE RADI
  ngOnInit(): void {
    
    this.currentUserId = this.currentUserService.snapshot!.userId;
    this.organizersApiService.getByUserId(this.currentUserId).subscribe({
        next: (response) => {
          this.currentOrgId = response.id;
          this.eventsByOrganizerRequest.id = this.currentOrgId;
          console.log("currentOrgid",this.currentOrgId)
          this.loadEvents();
          this.loadTicketTypes();
          this.isEditMode = this.data.mode === 'edit';
          this.title = this.isEditMode ? 'Edit ticket' : 'New ticket';
          if (this.isEditMode && this.data.ticketId) {
            this.loadTicket(this.data.ticketId);
          }
          else {
            this.form = this.formService.createTicketForm();
            this.filteredEvents = this.form.get('eventId')!.valueChanges.pipe(
             startWith(''),
             map(value => this._filterEvents(value || '')),
            );
            this.filteredTicketTypes = this.form.get('ticketTypeId')!.valueChanges.pipe(
              startWith(''),
              map(value => this._filterTicketTypes(value || '')),
            );
          }
        },
        error: (err) => {
          console.error('GetByUserId error:', err);
        },
    }
  );
  }
  
  loadTicketTypes() {
    this.startLoading();
    this.mappedTicketTypes = new Map<number, string>;
    this.ticketTypesApi.list().subscribe({
      next: (response) => {
        response.items.forEach((ticketType: ListTicketTypesQueryDto) => {
          this.mappedTicketTypes.set(ticketType.id, ticketType.name);
          this.ticketTypes.push(ticketType.name);
        });
        this.stopLoading();
      },
      error: (err) => {
        console.error('Load events error:', err);
        this.stopLoading('Failed to load events');
      },
    });
  }
  private loadTicket(id: number): void {
    this.isLoading = true;
    console.log(this.mappedTicketTypes);
    this.api.getById(id).subscribe({
      next: (ticket) => {
        this.form = this.formService.createTicketForm(ticket);
        this.api.getById(this.data.ticketId!).subscribe({
          next: (ticket) => {
            const eName=ticket.event.name;
            const tTName=ticket.ticketType.name;
            this.form.get('eventId')!.setValue(eName);
            this.form.get('ticketTypeId')!.setValue(tTName);
          },
          error: (err) => {
            console.error('Load ticket error:', err);
            this.dialogRef.close(false);
          }
        });
          this.filteredEvents = this.form.get('eventId')!.valueChanges.pipe(
             startWith(''),
             map(value => this._filterEvents(value || '')),
          );
          this.filteredTicketTypes = this.form.get('ticketTypeId')!.valueChanges.pipe(
            startWith(''),
            map(value => this._filterTicketTypes(value || '')),
          );
        this.isLoading = false;
      },
      error: (err) => {
        this.toaster.error('Failed to load ticket');
        console.error('Load ticket error:', err);
        this.dialogRef.close(false);
      }
    });
  }
 
  private _filterEvents(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.events.filter(event => event.toLowerCase().includes(filterValue));
  }
  private _filterTicketTypes(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.ticketTypes.filter(ticketType => ticketType.toLowerCase().includes(filterValue));
  }
  
  protected override loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: (err) => {
        console.error('Load tickets error:', err);
        this.stopLoading('Failed to load tickets');
      },
    });
  }

  protected loadEvents(): void {
    this.startLoading();
    this.mappedEvents = new Map<number, string>;
    console.log("organizer req",this.eventsByOrganizerRequest);
    this.eventsApi.getByOrganizerId(
      this.eventsByOrganizerRequest
    ).subscribe({
      next: (response) => {
        response.items.forEach((event: GetEventsByOrganizerIdDto) => {
          this.mappedEvents.set(event.id, event.name);
          this.events.push(event.name);
        });
        this.stopLoading();
      },
      error: (err) => {
        console.error('Load events error:', err);
        this.stopLoading('Failed to load events');
      },
    });
  }
   onSubmit(): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }
    if(!this.checkEvent()){
      this.toaster.error('Event does not exist');
      return;
    }
    if(!this.checkTicketType()){
      this.toaster.error('Ticket type does not exist');
      return;
    }
    this.quantityInStock=this.form.value.quantityInStock;
    this.unitPrice=this.form.value.unitPrice;
    this.benefits=this.form.value.benefits.trim();
    this.isLoading = true;
    if (this.isEditMode && this.data.ticketId) {
      this.updateTicket();
    } else {
      this.createTicket();
    }
  }
  private createTicket(): void {
      const command: CreateTicketsCommand = {
        eventId: this.eventID,
        ticketTypeId: this.ticketTypeID,
        quantityInStock: this.quantityInStock,
        unitPrice: this.unitPrice,
        benefits: this.benefits,
      };
      this.api.create(command).subscribe({
        next: () => {

          this.toaster.success('Ticket created successfully');
          this.dialogRef.close(true); // Signal success
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Create ticket error:', err);
        }
      });
    }
  private updateTicket(): void {
      const command: UpdateTicketsCommand = {
        id: this.data.ticketId!,
        eventId: this.eventID,
        ticketTypeId: this.ticketTypeID,
        quantityInStock: this.quantityInStock,
        unitPrice: this.unitPrice,
        benefits: this.benefits
      };

      this.api.update(this.data.ticketId!, command).subscribe({
        next: () => {
          this.toaster.success('Ticket updated successfully');
          this.dialogRef.close(true); // Signal success
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Update ticket error:', err);
        }
      });
    }
  onCancel(): void {
    this.dialogRef.close(false);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
  checkEvent() : boolean{
    for (const [key, value] of this.mappedEvents) {
        if(value === this.form.value.eventId.trim()){
          this.eventID=key;
          return true;
        }
      }
    return false;
  }
  checkTicketType() : boolean{
    for (const [key, value] of this.mappedTicketTypes) {
        if(value === this.form.value.ticketTypeId.trim()){
          this.ticketTypeID=key;
        return true;
      }
    }
    return false;
  }
}

