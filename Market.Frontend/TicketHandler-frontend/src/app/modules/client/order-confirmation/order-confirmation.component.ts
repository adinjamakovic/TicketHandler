import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CartService } from '../../../core/services/cart/cart.service';
import { PaymentsApiService } from '../../../api-services/payments/payments-api.service';
import {
  ConfirmPaymentCommandDto,
  PaymentIntentStatus,
} from '../../../api-services/payments/payments-api.models';

@Component({
  selector: 'app-order-confirmation',
  standalone: false,
  templateUrl: './order-confirmation.component.html',
  styleUrl: './order-confirmation.component.scss',
})
export class OrderConfirmationComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private paymentsApi = inject(PaymentsApiService);
  private cart = inject(CartService);

  isLoading = true;
  result: ConfirmPaymentCommandDto | null = null;
  errorMessage: string | null = null;

  get currency(): string {
    return this.result?.currency || 'BAM';
  }

  /** True while Stripe is still settling — the buyer should check back rather than retry. */
  get isPending(): boolean {
    return (
      this.result?.paymentStatus === PaymentIntentStatus.Processing ||
      this.result?.paymentStatus === PaymentIntentStatus.RequiresAction
    );
  }

  get isFailed(): boolean {
    return !this.isLoading && !this.errorMessage && !this.result?.isPaid && !this.isPending;
  }

  ngOnInit(): void {
    void this.confirm();
  }

  backToEvents(): void {
    this.router.navigate(['/client']);
  }

  backToCheckout(): void {
    this.router.navigate(['/client/checkout']);
  }

  retry(): void {
    void this.confirm();
  }

  private async confirm(): Promise<void> {
    const paymentIntentId = this.route.snapshot.queryParamMap.get('payment_intent');

    if (!paymentIntentId) {
      this.isLoading = false;
      this.errorMessage = 'We could not tell which payment this was. Please check your orders.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    try {
      this.result = await firstValueFrom(this.paymentsApi.confirm({ paymentIntentId }));

      // The paid tickets are removed from the cart server-side; refresh the navbar badge.
      this.cart.load();
    } catch (error) {
      this.errorMessage =
        error instanceof HttpErrorResponse
          ? error.error?.message ?? 'We could not confirm your payment.'
          : 'We could not confirm your payment.';
    } finally {
      this.isLoading = false;
    }
  }
}
