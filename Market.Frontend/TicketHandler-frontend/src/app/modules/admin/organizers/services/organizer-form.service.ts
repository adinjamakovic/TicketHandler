import {inject, Injectable} from '@angular/core';
import {FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors} from '@angular/forms';
import {GetOrganizerByIdQueryDto} from '../../../../api-services/organizers/organizers-api.model';

@Injectable()
export class OrganizerFormService {
  private fb = inject(FormBuilder);

  private passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('userPassword')?.value;
    const passwordCheck = group.get('passwordCheck')?.value;

    if (!password || !passwordCheck) {
      return null;
    }

    return password === passwordCheck ? null : { passwordMismatch: true };
  }

  createOrganizerForm(organizer?: GetOrganizerByIdQueryDto): FormGroup {
    const isEdit = !!organizer;

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
      userEmail: [
        organizer?.user.email ?? '',
        [
          Validators.required,
          Validators.email
        ]
      ],
      cityId: [
        organizer?.cityId ?? null,
        [Validators.required]
      ],
      address: [organizer?.address ?? '', [Validators.required]],
      userFirstName: [organizer?.user.firstName ?? '', [Validators.required]],
      userLastName: [organizer?.user.lastName ?? '', [Validators.required]],
      userBirthdate: [organizer?.user.birthDate ?? '', [Validators.required]],
      userCityId: [organizer?.user.cityId ?? 1, [Validators.required]],
      userAddress: [organizer?.user.address ?? '', [Validators.required]],
      userGender: [organizer?.user.gender ?? '', [Validators.required]],
      userPhone: [organizer?.user.phone ??  null, [Validators.required]],
      userPassword: [null, isEdit ? [] : [Validators.required]],
      passwordCheck: [null, isEdit ? [] : [Validators.required]],
    }, { validators: this.passwordMatchValidator.bind(this) });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    if (controlName === 'passwordCheck') {
      const control = form.get(controlName);
      if(!control || !control.touched) return '';
      
      if (form.hasError('passwordMismatch')) {
        return 'Passwords do not match';
      }
      
      if(!control.errors) return '';
      
      const errors = control.errors;
      if(errors['required']) return 'This field is required';
      return 'Invalid value';
    }

    const control = form.get(controlName);
    if(!control || !control.errors || !control.touched) return '';

    const errors = control.errors;

    if(errors['required'])
      return 'This field is required';
    if(errors['minLength'])
      return `Minimum ${errors['minLength'].requiredLength} characters required`;
    if(errors['maxLength'])
      return `Maximum ${errors['maxLength'].requiredLength} characters required`;
    if(errors['email'])
      return `A valid email address is required`;

    return 'Invalid value';
  }
}
