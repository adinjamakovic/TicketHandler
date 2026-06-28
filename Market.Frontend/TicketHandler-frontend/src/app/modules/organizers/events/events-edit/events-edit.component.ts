import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { BaseFormComponent } from '../../../../core/components/base-classes/base-form-component';
import { GetEventByIdQueryDto, UpdateEventCommand } from '../../../../api-services/events/events-api.model';
import { EventsApiService } from '../../../../api-services/events/events-api.service';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { EventsFormService } from '../services/events-form.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToasterService } from '../../../../core/services/toaster.service';
import { PerformersApiService } from '../../../../api-services/performers/performers-api.service';
import { ListPerformersQueryDto } from '../../../../api-services/performers/performers-api.models';
import { VenuesApiService } from '../../../../api-services/venues/venues-api.service';
import { EventTypesApiService } from '../../../../api-services/event-types/event-types-api.service';
import { ListVenuesQueryDto } from '../../../../api-services/venues/venues-api.models';
import { ListEventTypesQueryDto } from '../../../../api-services/event-types/event-types-api.models';
import { DateAdapter, MAT_DATE_FORMATS, NativeDateAdapter } from '@angular/material/core';
import { largePaging } from '../../../../core/models/paging/paging-utils';
import { forkJoin } from 'rxjs';

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
  selector: 'app-events-edit',
  standalone: false,
  templateUrl: './events-edit.component.html',
  styleUrl: './events-edit.component.scss',
  providers: [
    EventsApiService,
    PerformersApiService,
    EventsFormService,
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventsEditComponent
  extends BaseFormComponent<GetEventByIdQueryDto>
  implements OnInit
{
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);
  private formService = inject(EventsFormService);
  private toaster = inject(ToasterService);
  private api = inject(EventsApiService);
  private venuesApi = inject(VenuesApiService);
  private eventTypesApi = inject(EventTypesApiService);
  private performersApi = inject(PerformersApiService);

  private eventId!: number;

  venues: ListVenuesQueryDto[] = [];
  eventTypes: ListEventTypesQueryDto[] = [];
  performers: ListPerformersQueryDto[] = [];

  timeSlots: string[] = Array.from({ length: 48 }, (_, i) => {
    const h = Math.floor(i / 2).toString().padStart(2, '0');
    const m = i % 2 === 0 ? '00' : '30';
    return `${h}:${m}`;
  });

  ngOnInit(): void {
    this.eventId = +this.route.snapshot.params['id'];
    this.initForm(true);
  }

  protected override initForm(isEdit: boolean): void {
    super.initForm(isEdit);
    this.form = this.formService.createEventsForm();
  }

  protected override loadData(): void {
    this.startLoading();

    forkJoin({
      event: this.api.getById(this.eventId),
      venues: this.venuesApi.list({ paging: largePaging }),
      eventTypes: this.eventTypesApi.list({ paging: largePaging }),
      performers: this.performersApi.list({ paging: largePaging })
    }).subscribe({
      next: ({event, venues, eventTypes, performers}) => {
        this.model = event;
        this.venues = venues.items;
        this.eventTypes = eventTypes.items;
        this.performers = performers.items;
        this.form = this.formService.createEventsForm(event);
        this.form.patchValue({ scheduledDate: new Date(event.scheduledDate) });
        this.stopLoading();
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.stopLoading("Failed to load event");
        this.toaster.error("Event not found");
        this.router.navigate(['organizer/events']);
        this.cdr.markForCheck();
        console.log("Load events error", err);
      }
    })
  }

  protected override save(): void {
    if (this.form.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const rawDate = this.form.value.scheduledDate as unknown as Date;
    const command: UpdateEventCommand = {
      name: this.form.value.name,
      description: this.form.value.description,
      scheduledDate: rawDate instanceof Date ? rawDate.toISOString() : rawDate,
      venueId: this.form.value.venueId,
      image: this.form.value.image,
      eventTypeId: this.form.value.eventTypeId,
      performers: this.form.value.performers,
    };

    this.api.update(this.eventId, command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Event updated successfully');
        this.router.navigate(['/organizer/events']);
      },
      error: (err) => {
        this.stopLoading('Failed to update event');
        this.cdr.markForCheck();
        console.error('Update event error:', err);
      },
    });
  }

  get items(): FormArray {
    return this.form.get('performers') as FormArray;
  }

  addItem(): void {
    const itemGroup = this.fb.group({
      id: [0],
      performerId: [0],
      timeStamp: [''],
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
