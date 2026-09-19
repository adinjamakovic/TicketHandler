import { Component, inject, OnInit } from '@angular/core';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { ListOrganizersQueryDto, ListOrganizersRequest } from '../../../api-services/organizers/organizers-api.model';
import { OrganizerApiService } from '../../../api-services/organizers/organizers-api.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { MatDialog } from '@angular/material/dialog';
import { ToasterService } from '../../../core/services/toaster.service';
import {Router} from '@angular/router';
import { DialogButton } from '../../shared/models/dialog-config.model';
import { CitiesApiService } from '../../../api-services/cities/cities-api.service';
import { ListCitiesQueryDto, ListCitiesRequest } from '../../../api-services/cities/cities-api.models';

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
  private citiesApi = inject(CitiesApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  displayedColums: string[] = ['name', 'description', 'userName', 'cityName', 'emailAddress', 'actions'];

  cities: ListCitiesQueryDto[] = [];

  constructor(){
    super();
    this.request = new ListOrganizersRequest();
    this.request.paging.pageSize = 5;
  }

  ngOnInit(): void {
    this.loadCities();
    this.initList();
  }

  private loadCities(): void {
    const citiesRequest = new ListCitiesRequest();
    citiesRequest.paging.pageSize = 100;

    this.citiesApi.list(citiesRequest).subscribe({
      next: (response) => this.cities = response.items,
      error: (err) => console.error('Load cities error:', err)
    });
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

  /** Re-runs the query from page one — used by every filter control. */
  onFilterChange(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onClearFilters(): void {
    this.request.search = null;
    this.request.city = null;
    this.request.email = null;
    this.request.hasEvents = null;
    this.onFilterChange();
  }

  get hasActiveFilters(): boolean {
    return !!(this.request.search
      || this.request.city
      || this.request.email
      || (this.request.hasEvents !== null && this.request.hasEvents !== undefined));
  }
}
