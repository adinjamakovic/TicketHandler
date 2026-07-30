import {inject, Injectable} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {GetEventByIdQueryDto, GetEventByIdQueryDtoPerformers} from '../../../../api-services/events/events-api.model';
import {toTimeSlot} from '../../../../core/utils/DateUtilities/time-utils';

@Injectable()
export class EventsFormService {
  private fb = inject(FormBuilder);

  createEventsForm(event?: GetEventByIdQueryDto) : FormGroup {
    return this.fb.group({
      name: [
        event?.name ?? '',
        [
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(100),
        ]
      ],
      description: [
        event?.description ?? '',
      ],
      scheduledDate: [
        event?.scheduledDate ?? '',
        [
          Validators.required
        ]
      ],
      image: [
        event?.image ?? null
      ],
      venueId: [
        event?.venueId ?? '',
        [
          Validators.required
        ]
      ],
      eventTypeId: [
        event?.eventTypeId ?? '',
        [
          Validators.required
        ]
      ],
      performers: this.fb.array(
        event?.performers?.map(p => this.createPerformerGroup(p)) ?? []
      )
    });
  }

  /**
   * One performer row. `timeStamp` is normalized to "HH:mm" so the value matches the
   * time slot options — the API returns a TimeOnly as "HH:mm:ss".
   */
  createPerformerGroup(performer?: GetEventByIdQueryDtoPerformers): FormGroup {
    return this.fb.group({
      id: [performer?.id ?? 0],
      performerId: [performer?.performerId ?? null, [Validators.required]],
      timeStamp: [toTimeSlot(performer?.timeStamp), [Validators.required]]
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.errors || !control.touched) {
      return '';
    }

    const errors = control.errors;

    if (errors['required']) {
      return 'This field is required';
    }
    if (errors['minlength']) {
      return `Minimum ${errors['minlength'].requiredLength} characters required`;
    }
    if (errors['maxlength']) {
      return `Maximum ${errors['maxlength'].requiredLength} characters allowed`;
    }

    return 'Invalid value';
  }
}
