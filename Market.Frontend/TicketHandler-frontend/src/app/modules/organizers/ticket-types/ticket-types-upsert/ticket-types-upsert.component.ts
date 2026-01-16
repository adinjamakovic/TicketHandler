import { Component, inject, NgModule, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { FormGroup } from '@angular/forms';
import { CreateTicketTypesCommand, UpdateTicketTypesCommand } from '../../../../api-services/ticket-types/ticket-types-api.model';
import { TicketTypesApiService } from '../../../../api-services/ticket-types/ticket-types-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { TicketTypesFormService } from '../services/ticket-types-form.service';


export interface TicketTypesDialogData {
  mode: 'create' | 'edit';
  typeId?: number;
}


@Component({
  selector: 'app-ticket-types-upsert',
  standalone: false,
  templateUrl: './ticket-types-upsert.component.html',
  styleUrl: './ticket-types-upsert.component.scss',
  providers: [TicketTypesFormService],
  
})

export class TicketTypesUpsertComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<TicketTypesUpsertComponent>);
  private data = inject<TicketTypesDialogData>(MAT_DIALOG_DATA);
  private api = inject(TicketTypesApiService);
  private formService = inject(TicketTypesFormService);
  private toaster = inject(ToasterService);

  form!: FormGroup;
  isLoading = false;
  isEditMode = false;
  title = '';

  ngOnInit(): void {
    this.isEditMode = this.data.mode === 'edit';
    this.title = this.isEditMode ? 'Edit type' : 'New type';
    if (this.isEditMode && this.data.typeId) {
      this.loadtype(this.data.typeId);
    } else {
      this.form = this.formService.createTypeForm();
    }
  }

  private loadtype(id: number): void {
    this.isLoading = true;

    this.api.getById(id).subscribe({
      next: (type) => {
        this.form = this.formService.createTypeForm(type);
        this.isLoading = false;
      },
      error: (err) => {
        this.toaster.error('Failed to load type');
        console.error('Load type error:', err);
        this.dialogRef.close(false);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    if (this.isEditMode && this.data.typeId) {
      this.updatetype();
    } else {
      this.createtype();
    }
  }

  private createtype(): void {
    const command: CreateTicketTypesCommand = {
      name: this.form.value.name.trim(),
      description: this.form.value.description.trim(),
    };
    this.api.create(command).subscribe({
      next: () => {
        this.toaster.success('Type created successfully');
        this.dialogRef.close(true); // Signal success
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Create type error:', err);
      }
    });
  }

  private updatetype(): void {
    const command: UpdateTicketTypesCommand = {
      id: this.data.typeId!,
      name: this.form.value.name.trim(),
      description: this.form.value.description.trim()
    };

    this.api.update(this.data.typeId!, command).subscribe({
      next: () => {
        this.toaster.success('Type updated successfully');
        this.dialogRef.close(true); // Signal success
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Update type error:', err);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
