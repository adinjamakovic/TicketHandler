import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { throwError } from 'rxjs';
import { catchError, switchMap, take } from 'rxjs/operators';
import { AuthFacadeService } from '../services/auth/auth-facade.service';
import { environment } from '../../../environments/environment';

/**
 * HTTP interceptor that:
 * 1. Adds Authorization header with the OIDC access token (Market.API calls only)
 * 2. Sends the user back to IdentityServer on 401
 *
 * Token renewal is handled by angular-auth-oidc-client (silent renew with a
 * rotating refresh token), so there is no manual refresh/retry queue here.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const auth = inject(AuthFacadeService);

    if (!isApiRequest(req.url)) {
        return next(req);
    }

    return auth.getAccessToken().pipe(
        take(1),
        switchMap((accessToken) => {
            const authReq = accessToken
                ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
                : req;

            return next(authReq).pipe(
                catchError((err) => {
                    if (err instanceof HttpErrorResponse && err.status === 401) {
                        auth.redirectToLogin();
                    }
                    return throwError(() => err);
                })
            );
        })
    );
};

function isApiRequest(url: string): boolean {
    return url.startsWith(environment.apiUrl);
}
