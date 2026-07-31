import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, ValidationErrors, Validators } from '@angular/forms';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { forkJoin } from 'rxjs';

import { BaseFormComponent } from '../../../core/components/base-classes/base-form-component';
import {
  GetPersonByIdQueryDto,
  UpdatePersonCommand,
} from '../../../api-services/person/person.api.model';
import { PersonApiService } from '../../../api-services/person/person.api.service';
import { CitiesApiService } from '../../../api-services/cities/cities-api.service';
import { ListCitiesQueryDto } from '../../../api-services/cities/cities-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { DdMmYyyyDateAdapter, DD_MM_YYYY_FORMATS } from '../../../core/utils/DateUtilities/datepicker-utils';
import { AdminSettingsFormService } from './services/admin-settings-form.service';

@Component({
  selector: 'app-admin-settings',
  standalone: false,
  templateUrl: './admin-settings.component.html',
  styleUrl: './admin-settings.component.scss',
  providers: [
    AdminSettingsFormService,
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS },
  ],
})
export class AdminSettingsComponent
  extends BaseFormComponent<GetPersonByIdQueryDto>
  implements OnInit
{
  private personApi = inject(PersonApiService);
  private citiesApi = inject(CitiesApiService);
  private formService = inject(AdminSettingsFormService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);

  genders = ['Male', 'Female', 'Prefer not to say'];
  cities: ListCitiesQueryDto[] = [];
  personId!: number;
  hidePassword = true;
  hidePasswordCheck = true;

  accountEmail = '';
  accountName = '';

  ngOnInit(): void {
    const user = this.currentUser.snapshot;

    if (!user) {
      this.stopLoading('You must be signed in to view your settings');
      return;
    }

    this.personId = user.userId;
    this.accountEmail = user.email;
    this.accountName = user.name;

    this.initForm(true);
  }

  protected override loadData(): void {
    this.startLoading();

    forkJoin({
      person: this.personApi.getById(this.personId),
      cities: this.citiesApi.list(),
    }).subscribe({
      next: ({ person, cities }) => {
        this.model = person;
        this.cities = cities.items;
        this.form = this.formService.createProfileForm(person);
        this.setupOptionalPasswordValidation();
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load your profile');
        this.toaster.error('Could not load your profile');
        console.error('Load admin settings error:', err);
      },
    });
  }

  protected override save(): void {
    if (this.form.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const password = this.form.value.password?.trim();

    const command: UpdatePersonCommand = {
      firstName: this.form.value.firstName,
      lastName: this.form.value.lastName,
      birthDate: this.toIsoDate(this.form.value.birthDate),
      cityId: this.form.value.cityId,
      address: this.form.value.address,
      gender: this.form.value.gender,
      phone: this.form.value.phone,
      email: this.form.value.email,
      password: password ? password : null,
    };

    this.personApi.update(this.personId, command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Settings saved successfully');

        // Reflect what was just persisted, and clear the password fields so the
        // next save doesn't resubmit them. The password is deliberately not kept.
        const { password: _password, ...profile } = command;
        this.model = profile;
        this.accountEmail = command.email;
        this.accountName = `${command.firstName} ${command.lastName}`;
        this.clearPasswordFields();
      },
      error: (err) => {
        this.stopLoading('Failed to save your settings');
        console.error('Update admin settings error:', err);
      },
    });
  }

  onReset(): void {
    if (!this.model || this.isLoading) {
      return;
    }

    this.form = this.formService.createProfileForm(this.model);
    this.setupOptionalPasswordValidation();
    this.errorMessage = null;
    this.hidePassword = true;
    this.hidePasswordCheck = true;
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }

  private clearPasswordFields(): void {
    this.form.patchValue({ password: null, passwordCheck: null });
    this.form.get('password')?.markAsUntouched();
    this.form.get('passwordCheck')?.markAsUntouched();
    this.hidePassword = true;
    this.hidePasswordCheck = true;
  }

  private toIsoDate(value: string | Date): string {
    return value instanceof Date ? value.toISOString() : value;
  }

  private setupOptionalPasswordValidation(): void {
    const passwordControl = this.form.get('password');
    const passwordCheckControl = this.form.get('passwordCheck');

    const applyValidators = (): void => {
      const password = passwordControl?.value;

      if (password) {
        passwordControl?.setValidators([
          Validators.minLength(AdminSettingsFormService.PASSWORD_MIN_LENGTH),
          Validators.pattern(AdminSettingsFormService.PASSWORD_PATTERN),
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
}
