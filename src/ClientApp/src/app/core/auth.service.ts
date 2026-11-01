import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, finalize, shareReplay, tap, throwError } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { TokenResponse } from './api.models';

const STORAGE_KEY = 'clientapp.auth';

function readStoredTokens(): TokenResponse | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as TokenResponse) : null;
  } catch {
    // Storage can be unavailable (private mode, blocked site data).
    return null;
  }
}

function writeStoredTokens(tokens: TokenResponse | null): void {
  try {
    if (tokens) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
  } catch {
    // Ignore: the session then lives only in memory.
  }
}

/**
 * Holds the bearer tokens issued by the WebApi Identity endpoints
 * (`POST /api/Users/login` and `POST /api/Users/refresh`).
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly tokens = signal<TokenResponse | null>(readStoredTokens());
  private refreshInFlight$: Observable<TokenResponse> | null = null;

  readonly isLoggedIn = computed(() => this.tokens() !== null);
  readonly accessToken = computed(() => this.tokens()?.accessToken ?? null);

  hasRefreshToken(): boolean {
    return !!this.tokens()?.refreshToken;
  }

  login(email: string, password: string): Observable<TokenResponse> {
    return this.http
      .post<TokenResponse>(`${API_BASE_URL}/Users/login`, { email, password })
      .pipe(tap(tokens => this.store(tokens)));
  }

  /**
   * Exchanges the refresh token for a new pair. Concurrent callers share one request,
   * because the WebApi rotates refresh tokens and a second use would be rejected.
   */
  refresh(): Observable<TokenResponse> {
    if (!this.refreshInFlight$) {
      const refreshToken = this.tokens()?.refreshToken ?? '';

      this.refreshInFlight$ = this.http
        .post<TokenResponse>(`${API_BASE_URL}/Users/refresh`, { refreshToken })
        .pipe(
          tap(tokens => this.store(tokens)),
          catchError(error => {
            this.logout();
            return throwError(() => error);
          }),
          finalize(() => (this.refreshInFlight$ = null)),
          shareReplay(1),
        );
    }

    return this.refreshInFlight$;
  }

  logout(): void {
    this.tokens.set(null);
    writeStoredTokens(null);
  }

  private store(tokens: TokenResponse): void {
    this.tokens.set(tokens);
    writeStoredTokens(tokens);
  }
}
