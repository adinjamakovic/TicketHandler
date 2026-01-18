import { inject, Injectable } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { GetTicketsByIdQueryDto } from "../../../../api-services/tickets/tickets-api.model";

/**
 * Service for creating and managing product Ticket forms.
 * Used in modal dialog for both create and edit operations.
 */
@Injectable()
export class TicketsFormService {
  private fb = inject(FormBuilder);

  /**
   * Create a product Ticket form with validation.
   * If Ticket data is provided, form is pre-filled (edit mode).
   */
  createTicketForm(tickets?: GetTicketsByIdQueryDto): FormGroup {
    return this.fb.group({
      id: [
        tickets?.id ?? '',
      ],
      eventId: [
        tickets?.event.id ?? '',
        [
          Validators.required,
        ]
      ],
      ticketTypeId: [
        tickets?.ticketType.id ?? '',
        [
          Validators.required,
        ]
      ],
      quantityInStock: [
        tickets?.quantityInStock ?? '',
        [
          Validators.required,Validators.min(5)
        ]
      ],
      unitPrice: [
        tickets?.unitPrice ?? '',
        [
          Validators.required,Validators.min(0)
        ]
      ],
      benefits: [
        tickets?.benefits ?? '',
        [
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
    if (errors['min']) {
      return `Minimum value is ${errors['min'].min}`;
    }
    if (errors['max']) {
      return `Maximum value is ${errors['max'].max}`;
    }

    return 'Invalid value';
  }
}
