import {Component, inject, OnInit} from '@angular/core';
import {BaseFormComponent} from '../../../../core/components/base-classes/base-form-component';
import {GetOrganizerByIdQueryDto} from '../../../../api-services/organizers/organizers-api.model';
import {OrganizerApiService} from '../../../../api-services/organizers/organizers-api.service';
import {PersonApiService} from '../../../../api-services/person/person.api.service';
import {Router} from '@angular/router';
import {ToasterService} from '../../../../core/services/toaster.service';
import {OrganizerFormService} from '../services/organizer-form.service';

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
    private userApi = inject(PersonApiService);
    private formService = inject(OrganizerFormService);
    private router = inject(Router);
    private toaster = inject(ToasterService);

    ngOnInit(): void {
     this.initForm(false);
    }


    protected override loadData(): void {
        throw new Error("Method not implemented.");
    }
    protected override save(): void {
        throw new Error("Method not implemented.");
    }

}
