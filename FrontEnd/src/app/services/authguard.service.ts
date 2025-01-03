import { inject, Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { AuthorizatinService } from './authorizationcalls.service';
import { UserProfileService } from './userprofile.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  private authorizationService = inject(AuthorizatinService);
  private userProfileService = inject(UserProfileService);
  private router = inject(Router);

  canActivate(): Observable<boolean> {
    return this.authorizationService.checkAuthorization().pipe(
      map((response : any) => {
        this.userProfileService.updateUserName(response.user);
        return true;
      }),
      catchError((error) => {
        this.router.navigate(['/login'])
        return of(false);
      })
    );
  }
}



