import { DestroyRef, inject, Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { catchError, map, Observable, of, throwError } from 'rxjs';
import { AuthorizatinService } from './authorizationcalls.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  private router = inject(Router);
  private authenticationService = inject(AuthorizatinService);
  private destroyRef = inject(DestroyRef);

  canActivate(): boolean {
    return true;
  }
}
