import { Component, inject, OnInit } from '@angular/core';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { forkJoin, switchMap } from 'rxjs';
import { BaseFormComponent } from '../../../core/components/base-classes/base-form-component';
import {
  GetOrganizerByIdQueryDto,
  UpdateOrganizerCommand,
} from '../../../api-services/organizers/organizers-api.model';
import { OrganizerApiService } from '../../../api-services/organizers/organizers-api.service';
import { CitiesApiService } from '../../../api-services/cities/cities-api.service';
import { ListCitiesQueryDto } from '../../../api-services/cities/cities-api.models';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../core/services/toaster.service';
import {
  DdMmYyyyDateAdapter,
  DD_MM_YYYY_FORMATS,
} from '../../../core/utils/DateUtilities/datepicker-utils';
import { OrganizerSettingsFormService } from './services/organizer-settings-form.service';

@Component({
  selector: 'app-organizer-settings',
  standalone: false,
  templateUrl: './organizer-settings.component.html',
  styleUrl: './organizer-settings.component.scss',
  providers: [
    OrganizerSettingsFormService,
    { provide: DateAdapter, useClass: DdMmYyyyDateAdapter },
    { provide: MAT_DATE_FORMATS, useValue: DD_MM_YYYY_FORMATS },
  ],
})
export class OrganizerSettingsComponent
  extends BaseFormComponent<GetOrganizerByIdQueryDto>
  implements OnInit
{
  private api = inject(OrganizerApiService);
  private citiesApi = inject(CitiesApiService);
  private formService = inject(OrganizerSettingsFormService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);

  genders = ['Male', 'Female', 'Prefer not to say'];
  cities: ListCitiesQueryDto[] = [];
  organizerId?: number;
  hideNewPassword = true;
  hidePasswordCheck = true;

  ngOnInit(): void {
    this.initForm(true);
  }

  protected override loadData(): void {
    const userId = this.currentUser.snapshot?.userId;

    if (!userId) {
      this.stopLoading('Could not determine the signed in user');
      return;
    }

    this.startLoading();

    // The organizer record is resolved from the signed in user, never from a
    // route parameter - the backend enforces the same ownership rule on update.
    this.api
      .getByUserId(userId)
      .pipe(
        switchMap((organizer) =>
          forkJoin({
            organizer: this.api.getById(organizer.id),
            cities: this.citiesApi.list(),
          })
        )
      )
      .subscribe({
        next: ({ organizer, cities }) => {
          this.model = organizer;
          this.organizerId = organizer.id;
          this.cities = cities.items;
          this.form = this.formService.createSettingsForm(organizer);
          this.stopLoading();
        },
        error: (err) => {
          this.stopLoading('Failed to load your organizer profile');
          this.toaster.error('Failed to load your organizer profile');
          console.error('Load organizer settings error:', err);
        },
      });
  }

  protected override save(): void {
    if (!this.organizerId || this.form.invalid || this.isLoading) {
      return;
    }

    this.startLoading();

    const logo = this.form.value.logo;
    const command: UpdateOrganizerCommand = {
      name: this.form.value.name,
      description: this.form.value.description,
      cityid: this.form.value.cityId,
      address: this.form.value.address,
      // Only a newly picked file is uploaded; an unchanged logo stays as it is.
      logo: logo instanceof File ? logo : null,
      user: {
        firstName: this.form.value.userFirstName,
        lastName: this.form.value.userLastName,
        birthDate: this.form.value.userBirthdate,
        cityId: this.form.value.userCityId,
        address: this.form.value.userAddress,
        gender: this.form.value.userGender,
        phone: this.form.value.userPhone,
        email: this.form.value.userEmail,
        password: this.form.value.newPassword ?? '',
      },
    };

    this.api.update(this.organizerId, command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Settings saved');
        this.form.patchValue({ newPassword: null, passwordCheck: null });
        this.form.get('newPassword')?.markAsUntouched();
        this.form.get('passwordCheck')?.markAsUntouched();
      },
      error: (err) => {
        this.stopLoading('Failed to save your settings');
        console.error('Update organizer settings error:', err);
      },
    });
  }

  onReset(): void {
    if (this.model) {
      this.form = this.formService.createSettingsForm(this.model);
      this.hideNewPassword = true;
      this.hidePasswordCheck = true;
    }
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
