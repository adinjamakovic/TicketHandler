import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganizersLayoutComponent } from './organizers-layout/organizers-layout.component';
import { TicketsComponent } from './tickets/tickets.component';
import { TicketTypesComponent } from './ticket-types/ticket-types.component';
import { OrganizerSettingsComponent } from './organizer-settings/organizer-settings.component';
import {EventsComponent} from './events/events.component';
import {EventNewsComponent} from './event-news/event-news.component';

const routes: Routes = [
  {
    path: '',
    component: OrganizersLayoutComponent,
    children: [
          //Tickets
          {
            path: 'tickets',
            component: TicketsComponent,
          },
          //Ticket types
          {
            path: 'ticket-types',
            component: TicketTypesComponent,
          },
          //Events
          {
            path: 'events',
            component: EventsComponent,
          },
          {
            path: 'event-news',
            component: EventNewsComponent,
          },
          {
            path: 'settings',
            component: OrganizerSettingsComponent,
          },
          // default organizers route → /organizers/settings
          {
            path: '',
            redirectTo: 'settings',
            pathMatch: 'full',
          },
        ],
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OrganizerRoutingModule {}


