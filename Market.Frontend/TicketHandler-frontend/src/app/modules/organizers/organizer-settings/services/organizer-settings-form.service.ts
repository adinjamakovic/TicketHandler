import { inject, Injectable } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { GetOrganizerByIdQueryDto } from '../../../../api-services/organizers/organizers-api.model';

/**
 * Builds the organizer self-service settings form.
 * Same fields as the admin organizer edit form, minus the admin-only concerns:
 * the organizer always edits their own record and the password is optional
 * (left empty = keep the current one).
 */
@Injectable()
export class OrganizerSettingsFormService {
  private fb = inject(FormBuilder);

  private static readonly PASSWORD_MIN_LENGTH = 8;
  private static readonly PASSWORD_PATTERN =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/;

  createSettingsForm(organizer: GetOrganizerByIdQueryDto): FormGroup {
    const form = this.fb.group({
      name: [
        organizer.name ?? '',
        [Validators.required, Validators.minLength(3), Validators.maxLength(50)]
      ],
      description: [organizer.description ?? '', [Validators.maxLength(500)]],
      cityId: [organizer.cityId ?? null, [Validators.required]],
      address: [organizer.address ?? '', [Validators.required, Validators.maxLength(200)]],
      logo: [organizer.logo ?? ''],
      userFirstName: [organizer.user.firstName ?? '', [Validators.required, Validators.maxLength(100)]],
      userLastName: [organizer.user.lastName ?? '', [Validators.required, Validators.maxLength(100)]],
      userBirthdate: [this.toDate(organizer.user.birthDate), [Validators.required]],
      userGender: [organizer.user.gender ?? '', [Validators.required]],
      userCityId: [organizer.user.cityId ?? null, [Validators.required]],
      userAddress: [organizer.user.address ?? '', [Validators.required, Validators.maxLength(200)]],
      userPhone: [organizer.user.phone ?? '', [Validators.required]],
      userEmail: [
        organizer.user.email ?? '',
        [Validators.required, Validators.email, Validators.maxLength(256)]
      ],
      newPassword: [null as string | null],
      passwordCheck: [null as string | null],
    });

    this.setupOptionalPasswordValidation(form);

    return form;
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.errors || !control.touched) return '';

    const errors = control.errors;

    if (errors['required']) {
      return controlName === 'passwordCheck'
        ? 'Please confirm your new password'
        : 'This field is required';
    }
    if (errors['passwordMismatch']) return 'Passwords do not match';
    if (errors['minlength'])
      return `Minimum ${errors['minlength'].requiredLength} characters required`;
    if (errors['maxlength'])
      return `Maximum ${errors['maxlength'].requiredLength} characters allowed`;
    if (errors['pattern'])
      return 'Password must include uppercase, lowercase, a number, and a special character';
    if (errors['email']) return 'A valid email address is required';

    return 'Invalid value';
  }

  /**
   * The password pair is only validated once the organizer starts typing a new
   * password - an untouched pair means "keep the current password".
   */
  private setupOptionalPasswordValidation(form: FormGroup): void {
    const passwordControl = form.get('newPassword');
    const passwordCheckControl = form.get('passwordCheck');

    const applyValidators = (): void => {
      const password = passwordControl?.value;

      if (password) {
        passwordControl?.setValidators([
          Validators.minLength(OrganizerSettingsFormService.PASSWORD_MIN_LENGTH),
          Validators.pattern(OrganizerSettingsFormService.PASSWORD_PATTERN),
        ]);
        passwordCheckControl?.setValidators([
          Validators.required,
          (control: AbstractControl): ValidationErrors | null =>
            !control.value || control.value === password ? null : { passwordMismatch: true },
        ]);
      } else {
        passwordControl?.clearValidators();
        passwordCheckControl?.clearValidators();
      }

      passwordControl?.updateValueAndValidity({ emitEvent: false });
      passwordCheckControl?.updateValueAndValidity({ emitEvent: false });
    };

    // Re-running applyValidators() also revalidates the confirmation control,
    // so a changed password never leaves a stale "passwords match" result.
    passwordControl?.valueChanges.subscribe(applyValidators);

    applyValidators();
  }

  private toDate(value: string | Date | null | undefined): Date | null {
    if (!value) return null;
    const date = value instanceof Date ? value : new Date(value);
    return isNaN(date.getTime()) ? null : date;
  }
}
