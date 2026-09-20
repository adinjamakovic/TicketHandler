import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../../core/services/cart/cart.service';
import { GetCartQueryDtoItem } from '../../../api-services/cart/cart-api.model';
import { ToasterService } from '../../../core/services/toaster.service';
import { DialogButton } from '../../shared/models/dialog-config.model';
import { DialogHelperService } from '../../shared/services/dialog-helper.service';

@Component({
  selector: 'app-cart',
  standalone: false,
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit {
  private cart = inject(CartService);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private router = inject(Router);

  items = this.cart.items;
  savedItems = this.cart.savedItems;
  isLoading = this.cart.isLoading;
  isEmpty = this.cart.isEmpty;
  itemCount = this.cart.itemCount;
  totalAmount = this.cart.totalAmount;
  hasSavedItems = this.cart.hasSavedItems;
  savedLineCount = this.cart.savedLineCount;

  /** Blocks the quantity controls while a line is being written to the server. */
  pendingTicketId: number | null = null;

  ngOnInit(): void {
    this.cart.load();
  }

  trackByTicketId(_index: number, item: GetCartQueryDtoItem): number {
    return item.ticketId;
  }

  increase(item: GetCartQueryDtoItem): void {
    if (item.quantity >= item.quantityInStock) {
      this.toaster.warning(`Only ${item.quantityInStock} ticket(s) left for this event.`);
      return;
    }

    this.setQuantity(item, item.quantity + 1);
  }

  decrease(item: GetCartQueryDtoItem): void {
    if (item.quantity <= 1) {
      return;
    }

    this.setQuantity(item, item.quantity - 1);
  }

  /** Dropping a line can't be undone, so it goes through a confirmation first. */
  remove(item: GetCartQueryDtoItem): void {
    this.dialogHelper.cart
      .confirmRemove(item.ticketType.name, item.isSavedForLater)
      .subscribe(result => {
        if (result?.button === DialogButton.DELETE) {
          this.performRemove(item);
        }
      });
  }

  private performRemove(item: GetCartQueryDtoItem): void {
    const where = item.isSavedForLater ? 'your saved items' : 'your cart';
    this.pendingTicketId = item.ticketId;

    this.cart.remove(item.ticketId).subscribe({
      next: () => {
        this.pendingTicketId = null;
        this.toaster.success(`${item.ticketType.name} removed from ${where}`);
      },
      error: () => {
        this.pendingTicketId = null;
        this.toaster.error(`Could not remove the ticket from ${where}`);
      },
    });
  }

  /** Parks a line: it keeps its quantity but stops counting towards the order. */
  saveForLater(item: GetCartQueryDtoItem): void {
    this.pendingTicketId = item.ticketId;

    this.cart.saveForLater(item.ticketId).subscribe({
      next: () => {
        this.pendingTicketId = null;
        this.toaster.success(`${item.ticketType.name} saved for later`);
      },
      error: err => {
        this.pendingTicketId = null;
        this.toaster.error(err?.error?.message ?? 'Could not save the ticket for later');
      },
    });
  }

  moveToCart(item: GetCartQueryDtoItem): void {
    this.pendingTicketId = item.ticketId;

    this.cart.moveToCart(item.ticketId).subscribe({
      next: () => {
        this.pendingTicketId = null;
        this.toaster.success(`${item.ticketType.name} moved back to your cart`);
      },
      // Stock can drop while a line sits on the shelf, so the server's reason matters here.
      error: err => {
        this.pendingTicketId = null;
        this.toaster.error(err?.error?.message ?? 'Could not move the ticket to your cart');
      },
    });
  }

  isSoldOut(item: GetCartQueryDtoItem): boolean {
    return item.quantity > item.quantityInStock;
  }

  continueShopping(): void {
    this.router.navigate(['/events']);
  }

  /** The order itself is submitted from the checkout page, not here. */
  checkout(): void {
    if (this.isEmpty()) {
      return;
    }

    this.router.navigate(['/client/checkout']);
  }

  goToEvent(item: GetCartQueryDtoItem): void {
    this.router.navigate(['/event', item.event.id]);
  }

  private setQuantity(item: GetCartQueryDtoItem, quantity: number): void {
    this.pendingTicketId = item.ticketId;

    this.cart.updateQuantity(item.ticketId, quantity).subscribe({
      next: () => {
        this.pendingTicketId = null;
      },
      error: () => {
        this.pendingTicketId = null;
        this.toaster.error('Could not update the quantity');
      },
    });
  }
}
