import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BaseFormComponent } from '../../../../core/components/base-classes/base-form-component';
import {
  GetOrganizerByIdQueryDto,
  UpdateOrganizerCommand,
} from '../../../../api-services/organizers/organizers-api.model';
import { OrganizerApiService } from '../../../../api-services/organizers/organizers-api.service';
import { CitiesApiService } from '../../../../api-services/cities/cities-api.service';
import { ListCitiesQueryDto } from '../../../../api-services/cities/cities-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';
import { OrganizerFormService } from '../services/organizer-form.service';

@Component({
  selector: 'app-organizers-edit',
  standalone: false,
  templateUrl: './organizers-edit.component.html',
  styleUrl: './organizers-edit.component.scss',
  providers: [OrganizerFormService],
})
export class OrganizersEditComponent
  extends BaseFormComponent<GetOrganizerByIdQueryDto>
  implements OnInit
{
  private api = inject(OrganizerApiService);
  private citiesApi = inject(CitiesApiService);
  private formService = inject(OrganizerFormService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toaster = inject(ToasterService);

  genders = ['Male', 'Female', 'Prefer not to say'];
  cities: ListCitiesQueryDto[] = [];
  organizerId!: number;
  hideUserPassword = true;
  hidePasswordCheck = true;

  private static readonly PASSWORD_MIN_LENGTH = 8;
  private static readonly PASSWORD_PATTERN =
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/;

  ngOnInit(): void {
    this.organizerId = +this.route.snapshot.params['id'];
    this.initForm(true);
  }

  protected override loadData(): void {
    this.startLoading();

    forkJoin({
      organizer: this.api.getById(this.organizerId),
      cities: this.citiesApi.list(),
    }).subscribe({
      next: ({ organizer, cities }) => {
        this.model = organizer;
        this.cities = cities.items;
        this.form = this.formService.createOrganizerForm(organizer);
        this.setupOptionalPasswordValidation();
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load organizer');
        this.toaster.error('Organizer not found');
        this.router.navigate(['/admin/organizers']);
        console.error('Load organizer error:', err);
      },
    });
  }

  protected override save(): void {
    if (this.form.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const password = this.form.value.userPassword ?? '';

    const command: UpdateOrganizerCommand = {
      name: this.form.value.name,
      description: this.form.value.description,
      cityid: this.form.value.cityId,
      address: this.form.value.address,
      user: {
        firstName: this.form.value.userFirstName,
        lastName: this.form.value.userLastName,
        birthDate: this.form.value.userBirthdate,
        cityId: this.form.value.userCityId,
        address: this.form.value.userAddress,
        gender: this.form.value.userGender,
        phone: this.form.value.userPhone,
        email: this.form.value.userEmail,
        password,
      },
    };

    this.api.update(this.organizerId, command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Organizer updated successfully');
        this.router.navigate(['/admin/organizers']);
      },
      error: (err) => {
        this.stopLoading('Failed to update organizer');
        console.error('Update organizer error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/organizers']);
  }

  getErrorMessage(controlName: string): string {
    if (controlName === 'userPassword') {
      const message = this.getUserPasswordErrorMessage();
      if (message) {
        return message;
      }
    }

    if (controlName === 'passwordCheck') {
      const message = this.getPasswordCheckErrorMessage();
      if (message) {
        return message;
      }
    }

    return this.formService.getErrorMessage(this.form, controlName);
  }

  private setupOptionalPasswordValidation(): void {
    const passwordControl = this.form.get('userPassword');
    const passwordCheckControl = this.form.get('passwordCheck');

    const applyValidators = (): void => {
      const password = passwordControl?.value;

      if (password) {
        passwordControl?.setValidators([
          Validators.minLength(OrganizersEditComponent.PASSWORD_MIN_LENGTH),
          Validators.pattern(OrganizersEditComponent.PASSWORD_PATTERN),
        ]);
        passwordCheckControl?.setValidators([
          Validators.required,
          (control: AbstractControl): ValidationErrors | null => {
            const confirm = control.value;
            if (!confirm) {
              return null;
            }
            return password === confirm ? null : { passwordMismatch: true };
          },
        ]);
      } else {
        passwordControl?.clearValidators();
        passwordCheckControl?.clearValidators();
      }

      passwordControl?.updateValueAndValidity({ emitEvent: false });
      passwordCheckControl?.updateValueAndValidity({ emitEvent: false });
    };

    const revalidateConfirmPassword = (): void => {
      passwordCheckControl?.updateValueAndValidity({ emitEvent: false });
    };

    passwordControl?.valueChanges.subscribe(() => {
      applyValidators();
      revalidateConfirmPassword();
    });
    passwordCheckControl?.valueChanges.subscribe(revalidateConfirmPassword);

    applyValidators();
  }

  private getPasswordCheckErrorMessage(): string {
    const control = this.form.get('passwordCheck');
    if (!control?.touched || !control.errors) {
      return '';
    }

    const errors = control.errors;
    if (errors['required']) {
      return 'Please confirm your password';
    }
    if (errors['passwordMismatch']) {
      return 'Passwords do not match';
    }

    return 'Invalid value';
  }

  private getUserPasswordErrorMessage(): string {
    const control = this.form.get('userPassword');
    if (!control?.touched || !control.errors) {
      return '';
    }

    const errors = control.errors;
    if (errors['minlength']) {
      return `Password must be at least ${OrganizersEditComponent.PASSWORD_MIN_LENGTH} characters`;
    }
    if (errors['pattern']) {
      return 'Password must include uppercase, lowercase, a number, and a special character';
    }

    return 'Invalid password';
  }
}
