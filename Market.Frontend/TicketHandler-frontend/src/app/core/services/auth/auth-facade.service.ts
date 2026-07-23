// src/app/core/services/auth/auth-facade.service.ts
import { Injectable, inject, signal, computed } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, of, switchMap, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { OidcSecurityService } from 'angular-auth-oidc-client';

import { CurrentUserDto } from './current-user.dto';
import { JwtPayloadDto } from './jwt-payload.dto';

/**
 * Glavni auth servis (façade) nad Duende IdentityServerom.
 * - login/logout ide kroz OIDC authorization code + PKCE (redirect na IdentityServer)
 * - tokene drži angular-auth-oidc-client (storage + silent renew), ne mi
 * - dekodira access token i drži CurrentUser kao signal
 *
 * Koristi se u:
 * - interceptoru (getAccessToken)
 * - guardovima (isAuthenticated, isAdmin)
 * - komponentama (login, logout, navbar)
 */
@Injectable({ providedIn: 'root' })
export class AuthFacadeService {
  private oidc = inject(OidcSecurityService);

  private static readonly RETURN_URL_KEY = 'auth.returnUrl';

  // === REACTIVE STATE: current user ===

  private _currentUser = signal<CurrentUserDto | null>(null);

  /** readonly signal za UI – čita se kao auth.currentUser() */
  currentUser = this._currentUser.asReadonly();

  /** computed signali nad current userom */
  isAuthenticated = computed(() => !!this._currentUser());
  isAdmin = computed(() => this._currentUser()?.isAdmin ?? false);
  isOrganiser = computed(() => this._currentUser()?.IsOrganiser ?? false);
  isUser = computed(() => this._currentUser()?.IsUser ?? false);

  constructor() {
    this.oidc.isAuthenticated$
      .pipe(
        switchMap(({ isAuthenticated }) =>
          isAuthenticated ? this.oidc.getAccessToken() : of(null)
        ),
        takeUntilDestroyed()
      )
      .subscribe((token) => this.setUserFromToken(token));
  }

  // =========================================================
  // PUBLIC API
  // =========================================================

  login(returnUrl?: string): void {
    if (returnUrl) {
      sessionStorage.setItem(AuthFacadeService.RETURN_URL_KEY, returnUrl);
    } else {
      sessionStorage.removeItem(AuthFacadeService.RETURN_URL_KEY);
    }

    this.oidc.authorize();
  }

  logout(): Observable<unknown> {
    return this.oidc.logoff().pipe(tap(() => this._currentUser.set(null)));
  }

  redirectToLogin(returnUrl?: string): void {
    this.login(returnUrl ?? this.currentPath());
  }

  consumeReturnUrl(): string | null {
    const url = sessionStorage.getItem(AuthFacadeService.RETURN_URL_KEY);
    sessionStorage.removeItem(AuthFacadeService.RETURN_URL_KEY);
    return url;
  }

  // =========================================================
  // GETTERI ZA INTERCEPTOR
  // =========================================================

  getAccessToken(): Observable<string> {
    return this.oidc.getAccessToken();
  }

  // =========================================================
  // PRIVATE HELPERS
  // =========================================================

  private setUserFromToken(token: string | null): void {
    if (!token) {
      this._currentUser.set(null);
      return;
    }

    try {
      const payload = jwtDecode<JwtPayloadDto>(token);

      this._currentUser.set({
        userId: Number(payload.sub),
        email: payload.email ?? '',
        name: payload.name ?? payload.email ?? '',
        isAdmin: payload.is_admin === 'true',
        IsOrganiser: payload.is_organiser === 'true',
        IsUser: payload.is_user === 'true',
      });
    } catch (error) {
      console.error('Failed to decode access token:', error);
      this._currentUser.set(null);
    }
  }

  private currentPath(): string {
    return `${window.location.pathname}${window.location.search}`;
  }
}
