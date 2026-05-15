
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

import { EventsComponent } from './events/events.component';
import { EventNewsComponent } from './event-news/event-news.component';
import { EventNewsUpsertComponent } from './event-news/event-news-upsert/event-news-upsert.component';


@NgModule({
  declarations: [
    OrganizersLayoutComponent,
    OrganizerSettingsComponent,
    TicketTypesUpsertComponent,
    TicketTypesComponent,
    TicketsComponent,
    TicketsUpsertComponent,
    EventsComponent,
    EventNewsComponent,
    EventNewsUpsertComponent,

  ],
  imports: [
    OrganizerRoutingModule,
    SharedModule,
    MatTabBody,
    MatDialogModule,
    
]
})
export class OrganizerModule { }
