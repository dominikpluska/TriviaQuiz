import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { DestroyRef, inject, Injectable } from '@angular/core';
import { RegisterModel } from '../models/register.model';
import { LoginModel } from '../models/login.model';
import { catchError, of, throwError } from 'rxjs';
import { error } from 'console';

@Injectable({ providedIn: 'root' })
export class AuthorizatinService {
  private htppClient: HttpClient = inject(HttpClient);
  private destroyRef = inject(DestroyRef);

  createAccount(registerModel: RegisterModel) {
    let UserDto = JSON.stringify(registerModel);
    return this.htppClient
      .post('https://localhost:7501/Register', UserDto)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
    // const subscription = this.htppClient
    //   .post('https://localhost:7501/Register', UserDto)
    //   .pipe(
    //     catchError((error) => {
    //       return throwError(() => new Error(error));
    //     })
    //   )
    //   .subscribe({
    //     next: () => console.log('response'),
    //   });
    // this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  logIn(loginModel: LoginModel) {
    let UserDto = JSON.stringify(loginModel);
    return this.htppClient.post('https://localhost:7501/Login', UserDto).pipe(
      catchError((error) => {
        const errorMessage = error.error;
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
