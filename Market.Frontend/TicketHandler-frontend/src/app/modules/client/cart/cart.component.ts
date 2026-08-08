import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../../core/services/cart/cart.service';
import { GetCartQueryDtoItem } from '../../../api-services/cart/cart-api.model';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-cart',
  standalone: false,
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit {
  private cart = inject(CartService);
  private toaster = inject(ToasterService);
  private router = inject(Router);

  items = this.cart.items;
  isLoading = this.cart.isLoading;
  isEmpty = this.cart.isEmpty;
  itemCount = this.cart.itemCount;
  totalAmount = this.cart.totalAmount;

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

  remove(item: GetCartQueryDtoItem): void {
    this.pendingTicketId = item.ticketId;

    this.cart.remove(item.ticketId).subscribe({
      next: () => {
        this.pendingTicketId = null;
        this.toaster.success(`${item.ticketType.name} removed from your cart`);
      },
      error: () => {
        this.pendingTicketId = null;
        this.toaster.error('Could not remove the ticket from your cart');
      },
    });
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
