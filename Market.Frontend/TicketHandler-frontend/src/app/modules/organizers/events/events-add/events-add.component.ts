import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {form} from '@angular/forms/signals';
import {BaseFormComponent} from '../../../../core/components/base-classes/base-form-component';
import {
  CreateEventCommand,
  CreateEventCommandPerformer,
  GetEventByIdQueryDto
} from '../../../../api-services/events/events-api.model';
import {EventsApiService} from '../../../../api-services/events/events-api.service';
import {FormBuilder} from '@angular/forms';
import {EventsFormService} from '../services/events-form.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../../core/services/toaster.service';
import {PerformersApiService} from '../../../../api-services/performers/performers-api.service';
import {ListPerformersQueryDto, ListPerformersRequest} from '../../../../api-services/performers/performers-api.models';
import {VenuesApiService} from '../../../../api-services/venues/venues-api.service';
import {EventTypesApiService} from '../../../../api-services/event-types/event-types-api.service';
import {ListVenuesQueryDto} from '../../../../api-services/venues/venues-api.models';
import {ListEventTypesQueryDto} from '../../../../api-services/event-types/event-types-api.models';
import {provideNativeDateAdapter} from '@angular/material/core';
import {timestamp} from 'rxjs';

@Component({
  selector: 'app-events-add',
  standalone: false,
  templateUrl: './events-add.component.html',
  styleUrl: './events-add.component.scss',
  providers: [EventsApiService, PerformersApiService, EventsFormService, provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EventsAddComponent
  extends BaseFormComponent<GetEventByIdQueryDto>
  implements OnInit {
  private api = inject(EventsApiService);
  performersApi = inject(PerformersApiService);
  private venuesApi = inject(VenuesApiService);
  private eventTypesApi = inject(EventTypesApiService);
  private formService = inject(EventsFormService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  numberOfPerformers = 0;
  performers: ListPerformersQueryDto[] = [];
  venues: ListVenuesQueryDto[] = [];
  eventTypes: ListEventTypesQueryDto[] = [];
  selectedPerformers: CreateEventCommandPerformer[] = [];

  ngOnInit(): void {
    this.venues
    this.initForm(false);
    this.loadPerformers();
    this.loadVenues();
    this.loadEventTypes();
  }

  private loadVenues() {
        this.venuesApi.list().subscribe({
          next: (response) => {
            this.venues = response.items;
          },
          error: (error) => {
            this.toaster.error('Failed to load venues');
            console.error("Load venues error", error);
          }
        })
  }

  private loadEventTypes() {
    this.eventTypesApi.list().subscribe({
      next: (response) => {
        this.eventTypes = response.items;
      },
      error: (error) => {
        this.toaster.error('Failed to load event types');
        console.error("Load event types error", error);
      }
    })

  }

  private loadPerformers() {
    this.performersApi.list().subscribe({
      next: (response) => {
        this.performers = response.items;
      },
      error: (error) => {
        this.toaster.error('Failed to load event performers');
        console.error("Load event performers error", error);
      }
    })
  }

  protected override initForm(isEdit: boolean): void {
    super.initForm(isEdit);
    this.form = this.formService.createEventsForm();
  }

  protected override loadData(): void {

  }

  protected override save(): void {
      if(this.form.invalid || this.isLoading) {
        return;
      }

      this.startLoading();

      /*const command: CreateEventCommand = {
        name: this.form.value.name,
        description: this.form.value.description,
        scheduledDate: this.form.value.scheduledDate,
        venueId: this.form.value.venue.id,
        eventTypeId: this.form.value.eventType.id,

      }*/
  }

  findPerformer(performerId: number): string {
    for(var i = 0; i < this.performers.length; i++) {
      if(this.performers.at(i)!.id == performerId) {
        return this.performers.at(i)!.name;
      }
    }
    return 'Something';
  }

  AddPerformer(){
    const time: Date = this.form.value.time;
    var Time='';

    if(time)
    {
      const hours = time.getHours().toString().padStart(2, '0');
      const minutes = time.getMinutes().toString().padStart(2, '0');

      Time = `${hours}:${minutes}`;
    }
    if(this.selectedPerformers.includes(
      {
        performerId: this.form.value.performer,
        timeStamp: Time
      }
    ))
    {
      this.toaster.error("Cant add the same performer twice on the same time");
      throw new Error("Cant add the same performer");
    }
    this.selectedPerformers.push(
      {
        performerId: this.form.value.perfomer,
        timeStamp: Time
      }
    )
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }

  protected readonly onsubmit = onsubmit;

  protected readonly timestamp = timestamp;
}
