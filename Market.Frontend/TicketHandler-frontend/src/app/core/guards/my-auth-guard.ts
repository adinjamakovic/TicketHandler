// src/app/core/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { CurrentUserService } from '../services/auth/current-user.service';

export const myAuthGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const currentUser = inject(CurrentUserService);
  const router = inject(Router);

  // myAuthData() nests the flags under the 'auth' key
  const authData: MyAuthRouteData = route.data['auth'] ?? {};

  const requireAuth = authData.requireAuth === true;
  const requireAdmin = authData.requireAdmin === true;
  const requireOrganiser = authData.requireOrganiser === true;
  const requireUser = authData.requireUser === true;

  const isAuth = currentUser.isAuthenticated();

  // 1) if the route requires auth and the user is not logged in → login (redirect to IdentityServer)
  if (requireAuth && !isAuth) {
    router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // If auth is not required → allow (public routes)
  if (!requireAuth) {
    return true;
  }

  // 2) role check – admin > manager > employee
  const user = currentUser.snapshot;
  if (!user) {
    router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  if (requireAdmin && !user.isAdmin) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  if (requireOrganiser && !user.IsOrganiser) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  if (requireUser && !user.IsUser) {
    router.navigate([currentUser.getDefaultRoute()]);
    return false;
  }

  return true;
};

export interface MyAuthRouteData {
  requireAuth?: boolean;
  requireAdmin?: boolean;
  requireOrganiser?: boolean;
  requireUser?: boolean;
}

export function myAuthData(data: MyAuthRouteData): { auth: MyAuthRouteData } {
  return { auth: data };
}
