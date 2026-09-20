import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { CartApiService } from '../../../api-services/cart/cart-api.service';
import { GetCartQueryDtoItem } from '../../../api-services/cart/cart-api.model';
import { AuthFacadeService } from '../auth/auth-facade.service';

/*
 * Single source of truth for the cart across the whole app.
 * The cart itself lives on the server (Cart Items table, keyed by person),
 * this service just mirrors it as signals so the navbar badge, the event
 * details page and the cart screen never drift apart.
 */
@Injectable({ providedIn: 'root' })
export class CartService {
  private cartApi = inject(CartApiService);
  private auth = inject(AuthFacadeService);

  private _items = signal<GetCartQueryDtoItem[]>([]);
  private _savedItems = signal<GetCartQueryDtoItem[]>([]);
  private _isLoading = signal(false);

  items = this._items.asReadonly();
  savedItems = this._savedItems.asReadonly();
  isLoading = this._isLoading.asReadonly();

  itemCount = computed(() => this._items().reduce((sum, item) => sum + item.quantity, 0));
  lineCount = computed(() => this._items().length);
  totalAmount = computed(() => this._items().reduce((sum, item) => sum + item.subtotal, 0));
  isEmpty = computed(() => this._items().length === 0);
  savedLineCount = computed(() => this._savedItems().length);
  hasSavedItems = computed(() => this._savedItems().length > 0);

  constructor() {
    effect(() => {
      if (this.auth.isAuthenticated()) {
        this.load();
      } else {
        this._items.set([]);
        this._savedItems.set([]);
      }
    });
  }

  load(): void {
    if (!this.auth.isAuthenticated()) {
      return;
    }

    this._isLoading.set(true);
    this.cartApi.get().subscribe({
      next: cart => {
        this._items.set(cart.items ?? []);
        this._savedItems.set(cart.savedItems ?? []);
        this._isLoading.set(false);
      },
      error: () => {
        this._isLoading.set(false);
      },
    });
  }

  add(ticketId: number, quantity = 1): Observable<void> {
    return this.cartApi
      .addItem({ ticketId, quantity })
      .pipe(tap(() => this.load()));
  }

  updateQuantity(ticketId: number, quantity: number): Observable<void> {
    return this.cartApi
      .updateItem(ticketId, { quantity })
      .pipe(tap(() => this.load()));
  }

  remove(ticketId: number): Observable<void> {
    return this.cartApi
      .removeItem(ticketId)
      .pipe(tap(() => this.load()));
  }

  saveForLater(ticketId: number): Observable<void> {
    return this.cartApi
      .saveForLater(ticketId)
      .pipe(tap(() => this.load()));
  }

  moveToCart(ticketId: number): Observable<void> {
    return this.cartApi
      .moveToCart(ticketId)
      .pipe(tap(() => this.load()));
  }

  /** Empties the cart only — the saved-for-later shelf survives, so it has to be reloaded. */
  clear(): Observable<void> {
    return this.cartApi.clear().pipe(
      tap(() => {
        this._items.set([]);
        this.load();
      })
    );
  }
}
