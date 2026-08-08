import {NgModule} from '@angular/core';

import {ClientRoutingModule} from './client-routing-module';
import {SharedModule} from '../shared/shared-module';
import {CartComponent} from './cart/cart.component';
import {CheckoutComponent} from './checkout/checkout.component';
import {OrderConfirmationComponent} from './order-confirmation/order-confirmation.component';


@NgModule({
  declarations: [
    CartComponent,
    CheckoutComponent,
    OrderConfirmationComponent
  ],
  imports: [
    SharedModule,
    ClientRoutingModule
  ]
})
export class ClientModule { }
