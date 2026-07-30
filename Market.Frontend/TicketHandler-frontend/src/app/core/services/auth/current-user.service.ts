// src/app/core/services/auth/current-user.service.ts
import { Injectable, inject, computed } from '@angular/core';
import { AuthFacadeService } from './auth-facade.service';

@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  private auth = inject(AuthFacadeService);

  /** Signal the UI can read (readonly) */
  currentUser = computed(() => this.auth.currentUser());

  isAuthenticated = computed(() => this.auth.isAuthenticated());
  isAdmin = computed(() => this.auth.isAdmin());
  isOrganiser = computed(() => this.auth.isOrganiser());
  isUser = computed(() => this.auth.isUser());
  get snapshot() {
    return this.auth.currentUser();
  }

  /** Rule: admin > everyone else → client */
  getDefaultRoute(): string {
    const user = this.snapshot;
    if (!user) return '/auth/login';

    if (user.isAdmin) return '/admin';
    if (user.IsOrganiser) return '/organizer'
    return '/';
  }

  /**
   * The "Profile" route from the navbar dropdown - the portal matching the role.
   * Difference from getDefaultRoute(): a regular user goes to /client,
   * not to the landing page.
   */
  getProfileRoute(): string {
    const user = this.snapshot;
    if (!user) return '/auth/login';

    if (user.isAdmin) return '/admin';
    if (user.IsOrganiser) return '/organizer';
    return '/client';
  }
}
