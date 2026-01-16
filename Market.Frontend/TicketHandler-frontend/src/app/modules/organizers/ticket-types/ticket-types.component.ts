import { Component, inject, OnInit } from '@angular/core';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { ListTicketTypesQueryDto, ListTicketTypesRequest } from '../../../api-services/ticket-types/ticket-types-api.model';
import { TicketTypesApiService } from '../../../api-services/ticket-types/ticket-types-api.service';
import { MatDialog } from '@angular/material/dialog';
import { ToasterService } from '../../../core/services/toaster.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { TicketTypesUpsertComponent } from './ticket-types-upsert/ticket-types-upsert.component';
import { DialogButton } from '../../shared/models/dialog-config.model';

@Component({
  selector: 'app-ticket-types',
  standalone: false,
  templateUrl: './ticket-types.component.html',
  styleUrl: './ticket-types.component.scss',
})
export class TicketTypesComponent extends BaseListPagedComponent<ListTicketTypesQueryDto, ListTicketTypesRequest>
  implements OnInit
{
  private api = inject(TicketTypesApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);

  displayedColumns: string[] = ['name', 'description', 'actions'];
  showOnlyEnabled = true;

  constructor() {
    super();
    this.request = new ListTicketTypesRequest();
  }

  ngOnInit(): void {
    this.initList();
    
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load ticket types');
        console.error('Load ticket types error:', err);
      },
    });
  }

  // === Filters ===

  onSearchChange(searchTerm: string): void {
    this.request.search = searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onToggleEnabledFilter(checked: boolean): void {
    this.showOnlyEnabled = checked;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  // === CRUD Actions ===

  onCreate(): void {
    const dialogRef = this.dialog.open(TicketTypesUpsertComponent, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: 'ticket-type-dialog',
      autoFocus: true,
      disableClose: false,
      data: {
        mode: 'create',
      },
    });

    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.dialogHelper.ticketTypes.showCreateSuccess().subscribe();
        this.loadPagedData();
      }
    });
  }

  onEdit(type: ListTicketTypesQueryDto): void {
    const dialogRef = this.dialog.open(TicketTypesUpsertComponent, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: 'ticket-type-dialog',
      autoFocus: true,
      disableClose: false,
      data: {
        mode: 'edit',
        typeId: type.id,
        name: type.name,
        description: type.description,
      },
    });
    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.dialogHelper.ticketTypes.showUpdateSuccess().subscribe();
        this.loadPagedData();
      }
    });
  }

  onDelete(type: ListTicketTypesQueryDto): void {
    this.dialogHelper.ticketTypes.confirmDelete(type.name).subscribe(result => {
      if (result && result.button === DialogButton.DELETE) {
        this.performDelete(type);
      }
    });
  }

  private performDelete(type: ListTicketTypesQueryDto): void {
    this.startLoading();

    this.api.delete(type.id).subscribe({
      next: () => {
        this.dialogHelper.ticketTypes.showDeleteSuccess().subscribe();
        this.loadPagedData();
      },
      error: (err) => {
        this.stopLoading();

        const errorMessage = this.extractErrorMessage(err);

        // Show error dialog instead of toast
        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'ticket_TYPES.DIALOGS.ERROR_DELETE'
        ).subscribe();

        console.error('Delete type error:', err);
      },
    });
  }


  /**
   * Extract user-friendly error message from HTTP error response
   */
  private extractErrorMessage(err: any): string | null {
    if (err?.error) {
      if (typeof err.error === 'string') {
        return err.error;
      }

      if (err.error.message) {
        return err.error.message;
      }

      if (err.error.title) {
        return err.error.title;
      }

      if (err.error.errors && typeof err.error.errors === 'object') {
        const errors = Object.values(err.error.errors).flat();
        if (errors.length > 0) {
          return errors.join(', ');
        }
      }
    }

    if (err?.message) {
      return err.message;
    }

    if (err?.statusText && err.statusText !== 'Unknown Error') {
      return err.statusText;
    }

    return null;
  }
}

