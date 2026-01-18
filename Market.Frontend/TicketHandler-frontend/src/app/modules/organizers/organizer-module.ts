
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
import { EventsComponent } from './events/events.component';
import { EventNewsComponent } from './event-news/event-news.component';
import { EventNewsUpsertComponent } from './event-news/event-news-upsert/event-news-upsert.component';
import { EventsAddComponent } from './events/events-add/events-add.component';
import { EventsEditComponent } from './events/events-edit/events-edit.component';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatTimepickerModule} from '@angular/material/timepicker';

@NgModule({
  declarations: [
    OrganizersLayoutComponent,
    OrganizerSettingsComponent,
    TicketTypesUpsertComponent,
    TicketTypesComponent,
    TicketsComponent,
    EventsComponent,
    EventNewsComponent,
    EventNewsUpsertComponent,
    EventsAddComponent,
    EventsEditComponent,

  ],
  imports: [
    OrganizerRoutingModule,
    SharedModule,
    MatTabBody,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatTimepickerModule
],
})
export class OrganizerModule { }
