import { Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { CartService } from '../../../../core/services/cart/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  private router = inject(Router);
  private currentUserService = inject(CurrentUserService);
  private auth = inject(AuthFacadeService);
  private cart = inject(CartService);

  currentUser = this.auth.currentUser;

  /** Badge on the cart icon — total tickets across all lines. */
  cartCount = this.cart.itemCount;

  profileRoute = computed(() => this.currentUserService.getProfileRoute());

  logout(): void {
    this.router.navigate(['/auth/logout']);
  }

  goToCart(): void {
    if (!this.currentUserService.isAuthenticated()) {
      this.auth.redirectToLogin('/client/cart');
      return;
    }

    this.router.navigate(['/client/cart']);
  }

}
