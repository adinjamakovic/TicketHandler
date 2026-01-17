import { Component, inject, OnInit } from '@angular/core';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { ListOrganizersQueryDto, ListOrganizersRequest } from '../../../api-services/organizers/organizers-api.model';
import { OrganizerApiService } from '../../../api-services/organizers/organizers-api.service';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';
import { MatDialog } from '@angular/material/dialog';
import { ToasterService } from '../../../core/services/toaster.service';
import {Router} from '@angular/router';

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
  private dialog = inject(MatDialog);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private router = inject(Router);
  displayedColums: string[] = ['name', 'description', 'userName', 'cityName', 'emailAddress'];

  constructor(){
    super();
    this.request = new ListOrganizersRequest();
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
        this.stopLoading('Failed to load Organizers');
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




}
