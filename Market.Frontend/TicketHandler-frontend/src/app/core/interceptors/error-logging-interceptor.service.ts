import { HttpInterceptorFn, HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import * as Sentry from '@sentry/angular';

/**
 * HTTP interceptor that logs errors.
 *
 * IMPORTANT: This interceptor only LOGS errors, it does NOT show toasters.
 * Toaster notifications should be handled in individual components where you
 * can provide context-specific error messages.
 *
 * Features:
 * - Logs all HTTP errors to console
 * - Reports server/network failures to Sentry
 * - Re-throws errors so components can handle them
 */
export const errorLoggingInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Log error to console
      console.error('HTTP Error:', {
        url: req.url,
        method: req.method,
        status: error.status,
        statusText: error.statusText,
        message: error.message,
        error: error.error,
        timestamp: new Date().toISOString()
      });

      logToSentry(error, req);

      // Re-throw error so components can handle it
      // Components should show appropriate toaster messages with context
      return throwError(() => error);
    })
  );
};

/**
 * Send server-side and network failures to Sentry.
 */
function logToSentry(error: HttpErrorResponse, req: HttpRequest<any>): void {
  const isServerError = error.status >= 500;
  const isNetworkError = error.status === 0;

  if (!isServerError && !isNetworkError) {
    Sentry.addBreadcrumb({
      category: 'http',
      level: 'warning',
      message: `${req.method} ${req.url} -> ${error.status}`,
      data: { status: error.status, traceId: error.error?.traceId }
    });
    return;
  }

  Sentry.captureException(error, {
    tags: {
      'http.status_code': String(error.status),
      'http.method': req.method,
      // Correlates the frontend issue with the backend event logged by MarketExceptionHandler.
      traceId: error.error?.traceId ?? 'unknown'
    },
    extra: {
      url: req.url,
      status: error.status,
      statusText: error.statusText,
      responseBody: error.error
    }
  });
}

/**
 * Helper: Get user-friendly error message
 * Can be used by components to extract error messages
 */
export function getErrorMessage(error: HttpErrorResponse): string {
  // Check if error has a custom message from backend
  if (error.error?.message) {
    return error.error.message;
  }

  // Check for validation errors
  if (error.error?.errors && typeof error.error.errors === 'object') {
    const errors = Object.values(error.error.errors).flat();
    if (errors.length > 0) {
      return errors.join(', ');
    }
  }

  // Check for title (ProblemDetails format)
  if (error.error?.title) {
    return error.error.title;
  }

  // Fallback to generic messages based on status code
  switch (error.status) {
    case 0:
      return 'Unable to connect to server. Please check your internet connection.';
    case 400:
      return 'Invalid request. Please check your input.';
    case 401:
      return 'Unauthorized. Please log in again.';
    case 403:
      return 'You do not have permission to perform this action.';
    case 404:
      return 'The requested resource was not found.';
    case 409:
      return 'Conflict. The operation cannot be completed.';
    case 500:
      return 'Server error. Please try again later.';
    case 503:
      return 'Service temporarily unavailable. Please try again later.';
    default:
      return `An error occurred: ${error.statusText || 'Unknown error'}`;
  }
}
