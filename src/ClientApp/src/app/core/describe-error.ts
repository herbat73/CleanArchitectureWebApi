import { HttpErrorResponse } from '@angular/common/http';

/**
 * Turns an HTTP error from the WebApi into a message for the user.
 * The WebApi answers with ProblemDetails, optionally with per-field `errors`.
 */
export function describeError(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Something went wrong.';
  }

  if (error.status === 0) {
    return 'Cannot reach the WebApi. Is it running on http://localhost:5117?';
  }

  const body = error.error as { title?: string; detail?: string; errors?: Record<string, string[]> } | null;

  if (body?.errors) {
    const messages = Object.values(body.errors).flat();
    if (messages.length > 0) {
      return messages.join(' ');
    }
  }

  return body?.detail ?? body?.title ?? `Request failed (${error.status}).`;
}
