import { Component, inject, OnInit } from '@angular/core';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { ListOrganizersQueryDto, ListOrganizersRequest } from '../../../api-services/organizers/organizers-api.model';
import { OrganizerApiService } from '../../../api-services/organizers/organizers-api.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { MatDialog } from '@angular/material/dialog';
import { ToasterService } from '../../../core/services/toaster.service';
import {Router} from '@angular/router';
import { DialogButton } from '../../shared/models/dialog-config.model';

@Component({
  selector: 'app-organizers',
  standalone: false,
  templateUrl: './organizers.component.html',
  styleUrl: './organizers.component.scss',
})
export class OrganizersComponent
  extends BaseListPagedComponent<ListOrganizersQueryDto, ListOrganizersRequest>
  implements OnInit
{
  private api = inject(OrganizerApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  displayedColums: string[] = ['name', 'description', 'userName', 'cityName', 'emailAddress', 'actions'];

  constructor(){
    super();
    this.request = new ListOrganizersRequest();
    this.request.paging.pageSize = 5;
  }

  ngOnInit(): void {
    this.initList();
  }

  protected override loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (response)=>{
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load organizers');
        console.error("Load organizers error:", err);
      },
    });
  }

  onSearchChange(searchTerm: string): void {
    this.request.search = searchTerm;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onCreate(): void {
    this.router.navigate(['/admin/organizers/add']);
  }

  onEdit(organizer: ListOrganizersQueryDto): void {
    this.router.navigate(['/admin/organizers', organizer.id, 'edit']);
  }

  onDelete(organizer: ListOrganizersQueryDto): void {
    this.dialogHelper.organizers.confirmDelete(organizer.name).subscribe(result => {
      if (result && result.button === DialogButton.DELETE) {
        this.performDelete(organizer);
      }
    });
  }

  private performDelete(organizer: ListOrganizersQueryDto): void {
    this.startLoading();

    this.api.delete(organizer.id).subscribe({
      next: () => {
        this.dialogHelper.organizers.showDeleteSuccess().subscribe();
        this.loadPagedData();
      },
      error: (err) => {
        this.stopLoading();

        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'PRODUCTS.DIALOGS.ERROR_DELETE'
        ).subscribe();

        console.error('Delete organizer error:', err);
      }
    });
  }




  onSearch(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }
}
