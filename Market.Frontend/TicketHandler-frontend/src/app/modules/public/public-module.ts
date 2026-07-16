import {NgModule} from '@angular/core';

import {PublicRoutingModule} from './public-routing-module';
import {PublicLayoutComponent} from './public-layout/public-layout.component';
import {SearchProductsComponent} from './search-products/search-products.component';
import {EventDetailsComponent} from './event-details/event-details.component';
import {SharedModule} from '../shared/shared-module';
import { MatIcon } from "@angular/material/icon";


@NgModule({
  declarations: [
    PublicLayoutComponent,
    SearchProductsComponent,
    EventDetailsComponent
  ],
  imports: [
    SharedModule,
    PublicRoutingModule,
    MatIcon
]
})
export class PublicModule { }
