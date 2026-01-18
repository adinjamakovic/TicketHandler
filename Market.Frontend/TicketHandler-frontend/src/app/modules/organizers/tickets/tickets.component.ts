import { Component, inject, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { ListTicketsRequest, ListTicketsQueryDto } from "../../../api-services/tickets/tickets-api.model";
import { TicketsApiService } from "../../../api-services/tickets/tickets-api.service";
import { ToasterService } from "../../../core/services/toaster.service";
import { DialogButton } from "../../shared/models/dialog-config.model";
import { DialogHelperService } from "../../shared/services/dialog-helper.service";
import { TicketsUpsertComponent } from "./tickets-upsert/tickets-upsert.component";
import { BaseListPagedComponent } from "../../../core/components/base-classes/base-list-paged-component";

@Component({
  selector: 'app-tickets',
  standalone: false,
  templateUrl: './tickets.component.html',
  styleUrl: './tickets.component.scss',
})
export class TicketsComponent  extends BaseListPagedComponent<ListTicketsQueryDto, ListTicketsRequest>
  implements OnInit{
 private api = inject(TicketsApiService);
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);

  displayedColumns: string[] = ['event.name', 'ticketTypeId', 'quantityInStock', 'unitPrice', 'benefits', 'actions'];
  showOnlyEnabled = true;

  constructor() {
    super();
    this.request = new ListTicketsRequest();
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
        this.stopLoading('Failed to load tickets');
        console.error('Load tickets error:', err);
      },
    });
  }

  // === Filters ===

  onSearchChange(searchTerm: string): void {
    this.request.eventName = searchTerm;
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
    const dialogRef = this.dialog.open(TicketsUpsertComponent, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: 'ticket-dialog',
      autoFocus: true,
      disableClose: false,
      data: {
        mode: 'create',
      },
    });

    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.dialogHelper.tickets.showCreateSuccess().subscribe();
        this.loadPagedData();
      }
    });
  }

  onEdit(ticket: ListTicketsQueryDto): void {
    const dialogRef = this.dialog.open(TicketsUpsertComponent, {
      width: '500px',
      maxWidth: '90vw',
      panelClass: 'ticket-dialog',
      autoFocus: true,
      disableClose: false,
      data: {
        mode: 'edit',
        ticketId: ticket.id,
        eventId:  ticket.event.id,
        ticketTypeId: ticket.ticketType.id,
        quantityInStock:  ticket.quantityInStock,
        unitPrice:  ticket.unitPrice,
        benefits: ticket.benefits
      },
    });
    dialogRef.afterClosed().subscribe((success: boolean) => {
      if (success) {
        this.dialogHelper.tickets.showUpdateSuccess().subscribe();
        this.loadPagedData();
      }
    });
  }

  onDelete(ticket: ListTicketsQueryDto): void {
    this.dialogHelper.tickets.confirmDelete(ticket.event.name).subscribe(result => {
      if (result && result.button === DialogButton.DELETE) {
        this.performDelete(ticket);
      }
    });
  }

  private performDelete(ticket: ListTicketsQueryDto): void {
    this.startLoading();

    this.api.delete(ticket.id).subscribe({
      next: () => {
        this.dialogHelper.tickets.showDeleteSuccess().subscribe();
        this.loadPagedData();
      },
      error: (err) => {
        this.stopLoading();

        const errorMessage = this.extractErrorMessage(err);

        // Show error dialog instead of toast
        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'TICKETS.DIALOGS.ERROR_DELETE'
        ).subscribe();

        console.error('Delete ticket error:', err);
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
