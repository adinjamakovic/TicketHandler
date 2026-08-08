// === ENUMS ===

export enum PaymentIntentStatus {
  RequiresPaymentMethod = 1,
  RequiresConfirmation = 2,
  RequiresAction = 3,
  Processing = 4,
  RequiresCapture = 5,
  Succeeded = 6,
  Canceled = 7,
}

// === QUERIES (READ) ===

export interface GetPaymentConfigQueryDto {
  publishableKey: string;
  /** ISO 4217 currency every charge is created in. */
  currency: string;
  /** False when the server has no Stripe keys — the card form stays hidden. */
  isConfigured: boolean;
}

export interface GetPaymentQuoteQueryDtoLine {
  ticketId: number;
  eventName: string;
  ticketTypeName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
  discountAmount: number;
  total: number;
}

export interface GetPaymentQuoteQueryDto {
  currency: string;
  lineCount: number;
  ticketCount: number;
  subtotal: number;
  discountAmount: number;
  total: number;
  lines: GetPaymentQuoteQueryDtoLine[];
}

// === COMMANDS (WRITE) ===

export interface CreatePaymentIntentCommandBillingDetails {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  addressLine1: string;
  addressLine2: string | null;
  city: string;
  state: string;
  postalCode: string;
  countryId: number;
}

export interface CreatePaymentIntentCommand {
  billingDetails: CreatePaymentIntentCommandBillingDetails;
  note?: string | null;
}

export interface CreatePaymentIntentCommandDto {
  orderId: number;
  paymentIntentId: string;
  clientSecret: string;
  amount: number;
  currency: string;
}

export interface ConfirmPaymentCommand {
  paymentIntentId: string;
}

export interface ConfirmPaymentCommandDto {
  orderId: number;
  paymentIntentId: string;
  paymentStatus: PaymentIntentStatus;
  orderStatus: number;
  isPaid: boolean;
  amount: number;
  currency: string;
  failureMessage: string | null;
}
