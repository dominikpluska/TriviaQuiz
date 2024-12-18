import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import {
  HttpHandlerFn,
  HttpRequest,
  provideHttpClient,
  withFetch,
  withInterceptors,
} from '@angular/common/http';

function ContentTypeInterceptor(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn
) {
  const requestClone = request.clone({
    headers: request.headers.set('Content-Type', 'application/json'),
    withCredentials: true,
  });
  return next(requestClone);
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([ContentTypeInterceptor]), withFetch()),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideClientHydration(),
  ],
};
