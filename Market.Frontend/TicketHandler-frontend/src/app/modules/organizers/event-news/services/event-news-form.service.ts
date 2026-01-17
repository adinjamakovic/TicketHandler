import {inject, Injectable} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {GetEventNewsByIdQueryDto} from '../../../../api-services/event-news/event-news-api.model';

@Injectable()
export class EventNewsService {
  private fb = inject(FormBuilder);

  createNewsForm(news?: GetEventNewsByIdQueryDto): FormGroup {
    return this.fb.group({
      header: [
        news?.header ?? '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(50),
        ]
      ]
    })
  }

  getErrorMessages(form: FormGroup, controlName: string): string {
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
