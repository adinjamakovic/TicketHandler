import { Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';

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

  currentUser = this.auth.currentUser;

  profileRoute = computed(() => this.currentUserService.getProfileRoute());

  logout(): void {
    this.router.navigate(['/auth/logout']);
  }

}
