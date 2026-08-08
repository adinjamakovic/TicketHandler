import { Component, ElementRef, OnDestroy, OnInit, ViewChild, effect, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import {
  PaymentMethodCreateParams,
  StripeElements,
  StripePaymentElement,
} from '@stripe/stripe-js';
import { firstValueFrom } from 'rxjs';
import { CartService } from '../../../core/services/cart/cart.service';
import { StripeService } from '../../../core/services/payments/stripe.service';
import { GetCartQueryDtoItem } from '../../../api-services/cart/cart-api.model';
import { PaymentsApiService } from '../../../api-services/payments/payments-api.service';
import {
  CreatePaymentIntentCommandBillingDetails,
  GetPaymentQuoteQueryDto,
} from '../../../api-services/payments/payments-api.models';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { CountriesApiService } from '../../../api-services/countries/countries-api.service';
import {
  ListCountriesQueryDto,
  ListCountriesRequest,
} from '../../../api-services/countries/countries-api.models';

@Component({
  selector: 'app-checkout',
  standalone: false,
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private cart = inject(CartService);
  private paymentsApi = inject(PaymentsApiService);
  private stripeService = inject(StripeService);
  private countriesApi = inject(CountriesApiService);
  private auth = inject(AuthFacadeService);
  private toaster = inject(ToasterService);
  private router = inject(Router);

  items = this.cart.items;
  isLoading = this.cart.isLoading;
  isEmpty = this.cart.isEmpty;
  itemCount = this.cart.itemCount;

  countries: ListCountriesQueryDto[] = [];
  isLoadingCountries = false;
  isPlacingOrder = false;

  quote: GetPaymentQuoteQueryDto | null = null;
  currency = 'BAM';

  isPaymentReady = false;
  paymentUnavailableMessage: string | null = null;
  paymentError: string | null = null;

  private elements?: StripeElements;
  private paymentElement?: StripePaymentElement;
  private mountedHost?: HTMLElement;

  /**
   * Stays true once submit succeeds, so clearing the cart afterwards doesn't
   * trip the "empty cart" redirect below on its way to the confirmation.
   */
  private checkoutStarted = false;

  form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
    phone: ['', [Validators.required, Validators.maxLength(30)]],
    addressLine1: ['', [Validators.required, Validators.maxLength(200)]],
    addressLine2: ['', [Validators.maxLength(200)]],
    countryId: [null as number | null, [Validators.required]],
    city: ['', [Validators.required, Validators.maxLength(100)]],
    state: ['', [Validators.required, Validators.maxLength(100)]],
    postalCode: ['', [Validators.required, Validators.maxLength(20)]],
    note: ['', [Validators.maxLength(500)]],
    acceptedTerms: [false, [Validators.requiredTrue]],
  });

  @ViewChild('paymentElementHost')
  set paymentElementHost(host: ElementRef<HTMLDivElement> | undefined) {
    const element = host?.nativeElement;

    if (!element || element === this.mountedHost) {
      return;
    }

    this.mountedHost = element;
    void this.mountPaymentElement(element);
  }

  constructor() {
    effect(() => {
      if (!this.isLoading() && this.isEmpty() && !this.checkoutStarted) {
        this.router.navigate(['/client/cart']);
      }
    });
  }

  ngOnInit(): void {
    this.cart.load();
    this.loadCountries();
    this.prefillFromCurrentUser();
    void this.loadQuote();
  }

  ngOnDestroy(): void {
    this.paymentElement?.destroy();
  }

  /** Naming the amount on the button is a small anti-surprise measure before a charge. */
  get payButtonLabel(): string {
    if (!this.quote) {
      return 'Pay';
    }

    return `Pay ${this.quote.total.toFixed(2)} ${this.currency}`;
  }

  trackByTicketId(_index: number, item: GetCartQueryDtoItem): number {
    return item.ticketId;
  }

  hasError(controlName: string, errorType?: string): boolean {
    const control = this.form.get(controlName);
    if (!control || !control.touched) {
      return false;
    }

    return errorType ? control.hasError(errorType) : control.invalid;
  }

  backToCart(): void {
    this.router.navigate(['/client/cart']);
  }

  async submit(): Promise<void> {
    this.form.markAllAsTouched();
    this.paymentError = null;

    if (this.form.invalid || this.isEmpty() || this.isPlacingOrder) {
      return;
    }

    if (!this.elements || !this.isPaymentReady) {
      this.toaster.error('The payment form is not ready yet, please wait a moment');
      return;
    }

    this.isPlacingOrder = true;

    try {
      await this.pay(this.elements);
    } catch (error) {
      this.isPlacingOrder = false;
      this.checkoutStarted = false;
      this.showPaymentError(this.describeError(error, 'Could not take your payment, please try again'));
    }
  }

  private async pay(elements: StripeElements): Promise<void> {
    const stripe = await this.stripeService.stripe();

    if (!stripe) {
      this.isPlacingOrder = false;
      this.showPaymentError('Card payments are unavailable at the moment.');
      return;
    }

    // Runs the Payment Element's own validation before anything is created server-side.
    const submitResult = await elements.submit();

    if (submitResult.error) {
      this.isPlacingOrder = false;
      this.showPaymentError(submitResult.error.message ?? 'Please check your card details.');
      return;
    }

    // From here on an order exists, so the empty-cart redirect must stay out of the way.
    this.checkoutStarted = true;

    const value = this.form.getRawValue();
    const intent = await firstValueFrom(
      this.paymentsApi.createIntent({
        billingDetails: this.toBillingDetails(),
        note: value.note.trim() || null,
      })
    );

    // Keep the mounted amount in step with what the server actually charges — they can
    // differ if the cart changed in another tab while this page was open.
    if (this.quote && intent.amount !== this.quote.total) {
      elements.update(this.stripeService.elementsOptions(intent.amount, intent.currency));
      this.quote = { ...this.quote, total: intent.amount };
    }

    const { error, paymentIntent } = await stripe.confirmPayment({
      elements,
      clientSecret: intent.clientSecret,
      confirmParams: {
        // Only used by payment methods that leave the page (bank redirects, wallets).
        return_url: `${window.location.origin}/client/order-confirmation`,
        payment_method_data: {
          billing_details: this.toStripeBillingDetails(),
        },
      },
      // Cards stay in-page; anything needing a redirect still gets one.
      redirect: 'if_required',
    });

    if (error) {
      this.isPlacingOrder = false;
      this.checkoutStarted = false;
      this.showPaymentError(error.message ?? 'Your payment could not be completed.');
      return;
    }

    // Tell our backend right away so the buyer doesn't wait on the webhook.
    await this.settle(paymentIntent?.id ?? intent.paymentIntentId);
  }

  private async settle(paymentIntentId: string): Promise<void> {
    try {
      const result = await firstValueFrom(this.paymentsApi.confirm({ paymentIntentId }));

      this.cart.load();
      this.isPlacingOrder = false;

      if (result.isPaid) {
        this.toaster.success('Payment received — your tickets are confirmed');
      } else {
        this.toaster.info('Your payment is being processed');
      }

      this.router.navigate(['/client/order-confirmation'], {
        queryParams: { payment_intent: paymentIntentId },
      });
    } catch (error) {
      // The money may well have been taken — the webhook will still settle the order,
      // so send the buyer to the confirmation page rather than back to the form.
      this.isPlacingOrder = false;
      this.toaster.warning('Payment sent — confirming your order');

      this.router.navigate(['/client/order-confirmation'], {
        queryParams: { payment_intent: paymentIntentId },
      });
    }
  }

  private async mountPaymentElement(host: HTMLElement): Promise<void> {
    try {
      const stripe = await this.stripeService.stripe();

      if (!stripe) {
        this.paymentUnavailableMessage =
          'Card payments are not available right now. Please try again later.';
        return;
      }

      const quote = this.quote ?? (await this.loadQuote());

      if (!quote) {
        return;
      }

      this.elements = stripe.elements(
        this.stripeService.elementsOptions(quote.total, quote.currency)
      );

      this.paymentElement = this.elements.create('payment', {
        // The checkout form already asks for name and address; collecting them twice
        // would only give Stripe a second, possibly different answer.
        fields: { billingDetails: 'never' },
        // Link is what renders the "Save my information for a faster checkout" prompt.
        // We don't keep payment credentials on file, so the offer is hidden rather than
        // shown and ignored. Suppressed here rather than through the Elements group's
        // payment method types, which would have to stay in step with the intent.
        wallets: { link: 'never' },
      });

      this.paymentElement.mount(host);

      // Editing the card clears the previous decline message; whether the details are
      // complete is left to elements.submit() at pay time.
      this.paymentElement.on('change', () => (this.paymentError = null));
      this.paymentElement.on('ready', () => (this.isPaymentReady = true));
    } catch (error) {
      this.paymentUnavailableMessage = this.describeError(
        error,
        'The card form could not be loaded. Please refresh the page.'
      );
    }
  }

  private async loadQuote(): Promise<GetPaymentQuoteQueryDto | null> {
    try {
      const quote = await firstValueFrom(this.paymentsApi.quote());

      this.quote = quote;
      this.currency = quote.currency;

      return quote;
    } catch (error) {
      // An empty cart is the usual reason, and the effect above already redirects for it.
      if (!this.isEmpty()) {
        this.toaster.error(this.describeError(error, 'Could not price your cart'));
      }

      return null;
    }
  }

  private toBillingDetails(): CreatePaymentIntentCommandBillingDetails {
    const value = this.form.getRawValue();

    return {
      firstName: value.firstName.trim(),
      lastName: value.lastName.trim(),
      email: value.email.trim(),
      phone: value.phone.trim(),
      addressLine1: value.addressLine1.trim(),
      addressLine2: value.addressLine2.trim() || null,
      city: value.city.trim(),
      state: value.state.trim(),
      postalCode: value.postalCode.trim(),
      countryId: value.countryId!,
    };
  }

  /**
   * Same details in the shape Stripe wants, since the element collects none itself.
   *
   * Opting out of a field with `fields.billingDetails: 'never'` obliges us to supply it
   * here — and empty optionals must be sent as an explicit `null`, not `undefined`.
   * `undefined` drops the key from the object and Stripe reads that as "never passed",
   * which fails the confirm before it ever reaches the network. Address line 2 is the only
   * one that can still be empty; every other field the form requires.
   */
  private toStripeBillingDetails(): PaymentMethodCreateParams.BillingDetails {
    const value = this.form.getRawValue();
    const country = this.countries.find(c => c.id === value.countryId);

    return {
      name: `${value.firstName.trim()} ${value.lastName.trim()}`.trim(),
      email: value.email.trim(),
      phone: value.phone.trim(),
      address: {
        line1: value.addressLine1.trim(),
        line2: value.addressLine2.trim() || null,
        city: value.city.trim(),
        state: value.state.trim(),
        postal_code: value.postalCode.trim(),
        country: country?.isoCode ?? null,
      },
    };
  }

  private showPaymentError(message: string): void {
    this.paymentError = message;
    this.toaster.error(message);
  }

  private describeError(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse) {
      return error.error?.message ?? fallback;
    }

    return error instanceof Error ? error.message : fallback;
  }

  private loadCountries(): void {
    this.isLoadingCountries = true;

    this.countriesApi.list(new ListCountriesRequest()).subscribe({
      next: result => {
        this.countries = result.items ?? [];
        this.isLoadingCountries = false;
      },
      error: () => {
        this.isLoadingCountries = false;
        this.toaster.error('Could not load the country list');
      },
    });
  }

  private prefillFromCurrentUser(): void {
    const user = this.auth.currentUser();
    if (!user) {
      return;
    }

    const [firstName, ...rest] = (user.name ?? '').trim().split(/\s+/);

    this.form.patchValue({
      firstName: firstName ?? '',
      lastName: rest.join(' '),
      email: user.email ?? '',
    });
  }
}
