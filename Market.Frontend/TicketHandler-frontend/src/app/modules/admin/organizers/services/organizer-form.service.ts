import {inject, Injectable} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {GetOrganizerByIdQueryDto} from '../../../../api-services/organizers/organizers-api.model';

@Injectable()
export class OrganizerFormService {
  private fb = inject(FormBuilder);

  createOrganizerForm(organizer?: GetOrganizerByIdQueryDto) : FormGroup {
    return this.fb.group({
      name: [
        organizer?.name ?? '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50)
        ]
      ],
      description: [
        organizer?.description ?? '',
        [Validators.maxLength(500)]
      ],
      email: [
        organizer?.email ?? '',
        [
          Validators.required,
          Validators.email
        ]
      ],
      cityId: [
        organizer?.cityId ?? null,
        [Validators.required]
      ]
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if(!control || !control.errors || !control.touched) return '';

    const errors = control.errors;

    if(errors['required'])
      return 'This field is required';
    if(errors['minLength'])
      return `Minimum ${errors['minLength'].requiredLength} characters required`;
    if(errors['maxLength'])
      return `Minimum ${errors['maxLength'].requiredLength} characters required`;
    if(errors['email'])
      return `A valid email address is required`;

    return 'Invalid value';
  }
}
