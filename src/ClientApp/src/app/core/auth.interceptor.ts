import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { AuthService } from './auth.service';

const ACCOUNT_PREFIX = `${API_BASE_URL}/Users/`;

function withBearer(req: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

/**
 * Adds the bearer token to WebApi calls. When a call is rejected with 401 and a
 * refresh token is available, the tokens are refreshed once and the call is retried.
 * Identity endpoints (`/api/Users/...`) are sent untouched.
 */
export const authInterceptor: HttpInterceptorFn = (req, next: HttpHandlerFn) => {
  if (!req.url.startsWith(API_BASE_URL + '/') || req.url.startsWith(ACCOUNT_PREFIX)) {
    return next(req);
  }

  const auth = inject(AuthService);
  const token = auth.accessToken();

  return next(token ? withBearer(req, token) : req).pipe(
    catchError(error => {
      if (!(error instanceof HttpErrorResponse) || error.status !== 401 || !auth.hasRefreshToken()) {
        return throwError(() => error);
      }

      return auth.refresh().pipe(
        switchMap(tokens => next(withBearer(req, tokens.accessToken))),
      );
    }),
  );
};
