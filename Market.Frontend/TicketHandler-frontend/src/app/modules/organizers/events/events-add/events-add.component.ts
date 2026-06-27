import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {BaseFormComponent} from '../../../../core/components/base-classes/base-form-component';
import {
  CreateEventCommand,
  GetEventByIdQueryDto
} from '../../../../api-services/events/events-api.model';
import {EventsApiService} from '../../../../api-services/events/events-api.service';
import {FormArray, FormBuilder, FormGroup} from '@angular/forms';
import {EventsFormService} from '../services/events-form.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../../core/services/toaster.service';
import {PerformersApiService} from '../../../../api-services/performers/performers-api.service';
import {ListPerformersQueryDto} from '../../../../api-services/performers/performers-api.models';
import {VenuesApiService} from '../../../../api-services/venues/venues-api.service';
import {EventTypesApiService} from '../../../../api-services/event-types/event-types-api.service';
import {ListVenuesQueryDto} from '../../../../api-services/venues/venues-api.models';
import {ListEventTypesQueryDto} from '../../../../api-services/event-types/event-types-api.models';
import {DateAdapter, MAT_DATE_FORMATS, NativeDateAdapter} from '@angular/material/core';
import { largePaging } from '../../../../core/models/paging/paging-utils';

class DdMmYyyyDateAdapter extends NativeDateAdapter {
  override format(date: Date, _displayFormat: any): string {
    const d = date.getDate().toString().padStart(2, '0');
    const m = (date.getMonth() + 1).toString().padStart(2, '0');
    return `${d}/${m}/${date.getFullYear()}`;
  }
}

const DD_MM_YYYY_FORMATS = {
  parse: { dateInput: 'DD/MM/YYYY' },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'DD/MM/YYYY',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-events-add',
  standalone: false,
  templateUrl: './events-add.component.html',
  styleUrl: './events-add.component.scss',
  providers: [
    EventsApiService,
    PerformersApiService,
    EventsFormService,
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EventsAddComponent
  extends BaseFormComponent<GetEventByIdQueryDto>
  implements OnInit {
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private formService = inject(EventsFormService);
  private toaster = inject(ToasterService);
  private api = inject(EventsApiService);
  private venuesApi = inject(VenuesApiService);
  private eventTypesApi = inject(EventTypesApiService);
  private performersApi = inject(PerformersApiService);

  venues: ListVenuesQueryDto[] = [];
  eventTypes: ListEventTypesQueryDto[] = [];
  performers: ListPerformersQueryDto[] = [];
  timeSlots: string[] = Array.from({ length: 48 }, (_, i) => {
    const h = Math.floor(i / 2).toString().padStart(2, '0');
    const m = i % 2 === 0 ? '00' : '30';
    return `${h}:${m}`;
  });

  ngOnInit(): void {
    this.initForm(false);
    this.loadVenues();
    this.loadEventTypes();
    this.loadPerformers();
    this.addItem();
    this.addItem();
  }

  private loadVenues(): void {
    this.venuesApi.list({ paging: largePaging }).subscribe({
      next: (response) => {
        this.venues = response.items;
      },
      error: (err) => {
        this.toaster.error('Failed to load venues');
        console.error('Load venues error:', err);
      }
    });
  }

  private loadEventTypes(): void {
    this.eventTypesApi.list({ paging: largePaging }).subscribe({
      next: (response) => {
        this.eventTypes = response.items;
      },
      error: (err) => {
        this.toaster.error('Failed to load event types');
        console.error('Load event types error:', err);
      }
    });
  }

  private loadPerformers(): void {
    this.performersApi.list({ paging: largePaging }).subscribe({
      next: (response) => {
        this.performers = response.items;
      },
      error: (err) => {
        this.toaster.error('Failed to load event types');
        console.error('Load event types error:', err);
      }
    });
  }

  protected override initForm(isEdit: boolean): void {
    super.initForm(isEdit);
    this.form = this.formService.createEventsForm();
  }
  protected override loadData(): void {
  }

  protected override save(): void {
    if (this.form.invalid || this.isLoading) {
        return;
      }

      this.startLoading();

      const rawDate = this.form.value.scheduledDate as unknown as Date;
      const command: CreateEventCommand =
      {
        name: this.form.value.name,
        description: this.form.value.description,
        scheduledDate: rawDate instanceof Date ? rawDate.toISOString() : rawDate,
        venueId: this.form.value.venueId,
        image: this.form.value.image,
        eventTypeId: this.form.value.eventTypeId,
        performers: this.form.value.performers
      };

      console.log(command);

      this.api.create(command).subscribe({
        next: (eventId) => {
          this.stopLoading();
          this.toaster.success('Event created successfully');
          this.router.navigate(['/organizer/events']);
        },
        error: (err) => {
          this.stopLoading('Failed to create event');
          console.error('Create event error:', err);
          this.router.navigate(['/organizer/events']);
        }
      });
  }


  get items(): FormArray {
    return this.form.get('performers') as FormArray;
  }

  addItem(): void {
    const itemGroup = this.fb.group({
      performerId: [0],
      timeStamp: ['']
    });
    this.items.push(itemGroup);
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
  }

  onCancel(): void {
    this.router.navigate(['/organizer/events']);
  }

   getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }

}
