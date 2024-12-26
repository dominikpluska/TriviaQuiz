import { inject, Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AuthorizatinService } from './authorizationcalls.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  private authorizationService = inject(AuthorizatinService);

  canActivate(): Observable<boolean> {
    return this.authorizationService.checkAuthorization().pipe(
      map((response) => {
        return true;
      }),
      catchError((error) => {
        return of(false);
      })
    );
  }
}
