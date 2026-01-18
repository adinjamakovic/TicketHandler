import {inject, Injectable} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {GetEventByIdQueryDto} from '../../../../api-services/events/events-api.model';

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
      venue: [
        event?.venueName ?? '',
        [
          Validators.required
        ]
      ],
      eventType: [
        event?.eventTypeName ?? '',
        [
          Validators.required
        ]
      ],
      performer: [

      ],
      time: [

      ]
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
