import {inject, Injectable} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators} from '@angular/forms';
import {GetPersonByIdQueryDto} from '../../../../api-services/person/person.api.model';

@Injectable()
export class AdminSettingsFormService {
  private fb = inject(FormBuilder);

  static readonly PASSWORD_MIN_LENGTH = 8;
  static readonly PASSWORD_PATTERN =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/;

  private passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const passwordCheck = group.get('passwordCheck')?.value;

    if (!password || !passwordCheck) {
      return null;
    }

    return password === passwordCheck ? null : { passwordMismatch: true };
  }

  createProfileForm(person: GetPersonByIdQueryDto): FormGroup {
    return this.fb.group({
      firstName: [
        person.firstName ?? '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ],
      lastName: [
        person.lastName ?? '',
        [
          Validators.required,
          Validators.maxLength(100)
        ]
      ],
      birthDate: [person.birthDate ?? '', [Validators.required]],
      cityId: [person.cityId ?? null, [Validators.required]],
      address: [
        person.address ?? '',
        [
          Validators.required,
          Validators.maxLength(200)
        ]
      ],
      gender: [person.gender ?? '', [Validators.required]],
      phone: [person.phone ?? '', [Validators.required]],
      email: [
        person.email ?? '',
        [
          Validators.required,
          Validators.email,
          Validators.maxLength(256)
        ]
      ],
      // Optional - left blank the password is simply not changed.
      password: [null],
      passwordCheck: [null],
    }, { validators: this.passwordMatchValidator.bind(this) });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    if (controlName === 'passwordCheck') {
      const control = form.get(controlName);
      if (!control || !control.touched) return '';

      if (form.hasError('passwordMismatch')) {
        return 'Passwords do not match';
      }

      if (!control.errors) return '';

      if (control.errors['required']) return 'Please confirm your new password';
      return 'Invalid value';
    }

    const control = form.get(controlName);
    if (!control || !control.errors || !control.touched) return '';

    const errors = control.errors;

    if (errors['required'])
      return 'This field is required';
    if (errors['minlength'])
      return `Minimum ${errors['minlength'].requiredLength} characters required`;
    if (errors['maxlength'])
      return `Maximum ${errors['maxlength'].requiredLength} characters allowed`;
    if (errors['email'])
      return 'A valid email address is required';
    if (errors['pattern'])
      return 'Password must include uppercase, lowercase, a number, and a special character';

    return 'Invalid value';
  }
}
