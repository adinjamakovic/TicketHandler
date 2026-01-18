import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OrganizersLayoutComponent } from './organizers-layout/organizers-layout.component';
import { TicketsComponent } from './tickets/tickets.component';
import { TicketTypesComponent } from './ticket-types/ticket-types.component';
import { OrganizerSettingsComponent } from './organizer-settings/organizer-settings.component';
import { TicketsUpsertComponent } from './tickets/tickets-upsert/tickets-upsert.component';

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


