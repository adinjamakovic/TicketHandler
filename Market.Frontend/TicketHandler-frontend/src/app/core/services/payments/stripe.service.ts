import { inject, Injectable } from '@angular/core';
import { loadStripe, Stripe, StripeElementsOptionsMode } from '@stripe/stripe-js';
import { firstValueFrom } from 'rxjs';
import { PaymentsApiService } from '../../../api-services/payments/payments-api.service';
import { GetPaymentConfigQueryDto } from '../../../api-services/payments/payments-api.models';

/**
 * Owns the Stripe.js instance for the whole app.
 *
 * The publishable key comes from the backend rather than environment.ts so a build can
 * be pointed at a different Stripe account without being rebuilt, and so the frontend
 * never has to be kept in sync with the key the server actually charges with.
 */
@Injectable({ providedIn: 'root' })
export class StripeService {
  private paymentsApi = inject(PaymentsApiService);

  /** Both are cached as promises so concurrent callers share one request / one SDK load. */
  private configPromise?: Promise<GetPaymentConfigQueryDto>;
  private stripePromise?: Promise<Stripe | null>;

  config(): Promise<GetPaymentConfigQueryDto> {
    this.configPromise ??= firstValueFrom(this.paymentsApi.config());

    return this.configPromise;
  }

  async stripe(): Promise<Stripe | null> {
    const config = await this.config();

    if (!config.isConfigured || !config.publishableKey) {
      return null;
    }

    this.stripePromise ??= loadStripe(config.publishableKey);

    return this.stripePromise;
  }

  /**
   * Options for a deferred-intent Elements group: the Payment Element is mounted from
   * an amount alone, and the PaymentIntent is only created once the buyer submits. That
   * way visiting the checkout page does not leave a trail of abandoned orders.
   */
  elementsOptions(amount: number, currency: string): StripeElementsOptionsMode {
    return {
      mode: 'payment',
      amount: toMinorUnits(amount),
      currency: currency.toLowerCase(),
      appearance: {
        theme: 'stripe',
        variables: {
          colorPrimary: '#7b1fa2',
          colorText: '#1c1c1c',
          colorDanger: '#c62828',
          borderRadius: '8px',
          fontFamily: 'Roboto, "Helvetica Neue", sans-serif',
        },
      },
    };
  }
}

function toMinorUnits(amount: number): number {
  return Math.round(amount * 100);
}
