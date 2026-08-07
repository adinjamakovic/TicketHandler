import {NgModule} from '@angular/core';

import {ClientRoutingModule} from './client-routing-module';
import {SharedModule} from '../shared/shared-module';
import {CartComponent} from './cart/cart.component';


@NgModule({
  declarations: [
    CartComponent
  ],
  imports: [
    SharedModule,
    ClientRoutingModule
  ]
})
export class ClientModule { }
