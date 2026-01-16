import { Injectable, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GetTicketTypesByIdQueryDto } from '../../../../api-services/ticket-types/ticket-types-api.model';

/**
 * Service for creating and managing product Type forms.
 * Used in modal dialog for both create and edit operations.
 */
@Injectable()
export class TicketTypesFormService {
  private fb = inject(FormBuilder);

  /**
   * Create a product Type form with validation.
   * If Type data is provided, form is pre-filled (edit mode).
   */
  createTypeForm(type?: GetTicketTypesByIdQueryDto): FormGroup {
    return this.fb.group({
      name: [
        type?.name ?? '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(100)
        ],
      ],
      description: [
        type?.description ?? '',
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(100)
        ]
      ],
    });
  }

  /**
   * Get validation error message for a form control.
   */
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
