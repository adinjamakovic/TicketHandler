// src/app/core/services/auth/current-user.service.ts
import { Injectable, inject, computed } from '@angular/core';
import { AuthFacadeService } from './auth-facade.service';

@Injectable({ providedIn: 'root' })
export class CurrentUserService {
  private auth = inject(AuthFacadeService);

  /** Signal koji UI može čitati (readonly) */
  currentUser = computed(() => this.auth.currentUser());

  isAuthenticated = computed(() => this.auth.isAuthenticated());
  isAdmin = computed(() => this.auth.isAdmin());
  isOrganiser = computed(() => this.auth.isOrganiser());
  isUser = computed(() => this.auth.isUser());
  get snapshot() {
    return this.auth.currentUser();
  }

  /** Pravilo: admin > ostali → client */
  getDefaultRoute(): string {
    const user = this.snapshot;
    if (!user) return '/auth/login';

    if (user.isAdmin) return '/admin';
    if (user.IsOrganiser) return '/organizer'
    return '/';
  }

  /**
   * Ruta "Profile" iz navbar dropdowna – portal koji odgovara roli.
   * Razlika u odnosu na getDefaultRoute(): običan user ide na /client,
   * a ne na landing page.
   */
  getProfileRoute(): string {
    const user = this.snapshot;
    if (!user) return '/auth/login';

    if (user.isAdmin) return '/admin';
    if (user.IsOrganiser) return '/organizer';
    return '/client';
  }
}
