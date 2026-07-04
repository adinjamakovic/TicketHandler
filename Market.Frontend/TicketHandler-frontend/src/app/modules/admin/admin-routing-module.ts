import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { ProductsComponent } from './catalogs/products/products.component';
import { ProductsAddComponent } from './catalogs/products/products-add/products-add.component';
import { ProductsEditComponent } from './catalogs/products/products-edit/products-edit.component';
import { ProductCategoriesComponent } from './catalogs/product-categories/product-categories.component';
import {AdminOrdersComponent} from './orders/admin-orders.component';
import {AdminSettingsComponent} from './admin-settings/admin-settings.component';
import { OrganizersComponent } from './organizers/organizers.component';
import {OrganizersAddComponent} from './organizers/organizers-add/organizers-add.component';
import { OrganizersEditComponent } from './organizers/organizers-edit/organizers-edit.component';
import { EventsComponent } from './events/events.component';


const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      //ORGANIZERS
      {
        path: 'organizers',
        component: OrganizersComponent,
      },
      {
        path: 'organizers/add',
        component: OrganizersAddComponent,
      },
      {
        path: 'organizers/:id/edit',
        component: OrganizersEditComponent,
      },
      // EVENTS
      {
        path: 'events',
        component: EventsComponent
      },

      // default admin route → /admin/organizers
      {
        path: '',
        redirectTo: 'organizers',
        pathMatch: 'full',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
