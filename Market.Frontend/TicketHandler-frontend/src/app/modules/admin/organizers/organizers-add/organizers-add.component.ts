import {Component, inject, OnInit} from '@angular/core';
import {AbstractControl, ValidationErrors, Validators} from '@angular/forms';
import {BaseFormComponent} from '../../../../core/components/base-classes/base-form-component';
import {
  CreateOrganizerCommand,
  GetOrganizerByIdQueryDto
} from '../../../../api-services/organizers/organizers-api.model';
import {OrganizerApiService} from '../../../../api-services/organizers/organizers-api.service';
import {PersonApiService} from '../../../../api-services/person/person.api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../../core/services/toaster.service';
import {OrganizerFormService} from '../services/organizer-form.service';
import {CitiesApiService} from '../../../../api-services/cities/cities-api.service';
import {ListCitiesQueryDto} from '../../../../api-services/cities/cities-api.models';

@Component({
  selector: 'app-organizers-add',
  standalone: false,
  templateUrl: './organizers-add.component.html',
  styleUrl: './organizers-add.component.scss',
  providers: [OrganizerFormService],
})
export class OrganizersAddComponent
  extends BaseFormComponent<GetOrganizerByIdQueryDto>
  implements OnInit {
    private api = inject(OrganizerApiService);
    private citiesApi = inject(CitiesApiService);
    private formService = inject(OrganizerFormService);
    private router = inject(Router);
    private toaster = inject(ToasterService);
    genders = ['Male', 'Female', 'Prefer not to say'];
    cities: ListCitiesQueryDto[] = [];
    hideUserPassword = true;
    hidePasswordCheck = true;

    private static readonly PASSWORD_MIN_LENGTH = 8;
    private static readonly PASSWORD_PATTERN =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/;

    ngOnInit(): void {
     this.initForm(false);
     this.setupPasswordValidation();
     this.loadCities();
    }


    protected override loadData(): void {

    }
    protected override save(): void {
      if (this.form.invalid || this.isLoading) {
        return;
      }

      this.startLoading();

      const command: CreateOrganizerCommand = {
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
          password: this.form.value.userPassword
        }
      };

      this.api.create(command).subscribe({
        next: (productId) => {
          this.stopLoading();
          this.toaster.success('Organizer created successfully');
          this.router.navigate(['/admin/organizers']);
        },
        error: (err) => {
          this.stopLoading('Failed to create organizer');
          console.error('Create organizer error:', err);
        }
      });
    }

  private loadCities(): void {
    this.citiesApi.list().subscribe({
      next: (response) => {
        this.cities = response.items;
      },
      error: (err) => {
        this.toaster.error('Failed to load cities');
        console.error('Load cities error:', err);
      }
    });
  }

  protected override initForm(isEdit: boolean): void {
    super.initForm(isEdit);
    this.form = this.formService.createOrganizerForm();
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

  private setupPasswordValidation(): void {
    const passwordControl = this.form.get('userPassword');
    const passwordCheckControl = this.form.get('passwordCheck');

    passwordControl?.setValidators([
      Validators.required,
      Validators.minLength(OrganizersAddComponent.PASSWORD_MIN_LENGTH),
      Validators.pattern(OrganizersAddComponent.PASSWORD_PATTERN),
    ]);
    passwordControl?.updateValueAndValidity();

    passwordCheckControl?.setValidators([
      Validators.required,
      (control: AbstractControl): ValidationErrors | null => {
        const password = passwordControl?.value;
        const confirm = control.value;
        if (!password || !confirm) {
          return null;
        }
        return password === confirm ? null : { passwordMismatch: true };
      },
    ]);
    passwordCheckControl?.updateValueAndValidity();

    const revalidateConfirmPassword = (): void => {
      passwordCheckControl?.updateValueAndValidity({ emitEvent: false });
    };

    passwordControl?.valueChanges.subscribe(revalidateConfirmPassword);
    passwordCheckControl?.valueChanges.subscribe(revalidateConfirmPassword);
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
    if (errors['required']) {
      return 'Password is required';
    }
    if (errors['minlength']) {
      return `Password must be at least ${OrganizersAddComponent.PASSWORD_MIN_LENGTH} characters`;
    }
    if (errors['pattern']) {
      return 'Password must include uppercase, lowercase, a number, and a special character';
    }

    return 'Invalid password';
  }
}
