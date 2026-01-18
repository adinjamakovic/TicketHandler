
import { NgModule } from '@angular/core';
import {SharedModule} from '../shared/shared-module';
import { OrganizerRoutingModule } from './organizer-routing.module';
import { OrganizersLayoutComponent } from './organizers-layout/organizers-layout.component';
import { MatTabBody } from '@angular/material/tabs';
import { OrganizerSettingsComponent } from './organizer-settings/organizer-settings.component';
import { MatDialogModule } from '@angular/material/dialog';
import { TicketTypesUpsertComponent } from './ticket-types/ticket-types-upsert/ticket-types-upsert.component';
import { TicketTypesFormService } from './ticket-types/services/ticket-types-form.service';
import { TicketsComponent } from './tickets/tickets.component';
import { TicketTypesComponent } from './ticket-types/ticket-types.component';
import { TicketsUpsertComponent } from './tickets/tickets-upsert/tickets-upsert.component';
import { EventsApiService } from '../../api-services/events/events-api.service';



@NgModule({
  declarations: [
    OrganizersLayoutComponent,
    OrganizerSettingsComponent,
    TicketTypesUpsertComponent,
    TicketTypesComponent,
    TicketsComponent,
    TicketsUpsertComponent,
  ],
  imports: [
    OrganizerRoutingModule,
    SharedModule,
    MatTabBody,
    MatDialogModule,
    
]
})
export class OrganizerModule { }
