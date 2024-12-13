import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { DestroyRef, inject, Injectable } from '@angular/core';
import { RegisterModel } from '../models/register.model';
import { LoginModel } from '../models/login.model';
import { catchError, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthorizatinService {
  private htppClient: HttpClient = inject(HttpClient);
  private destroyRef = inject(DestroyRef);

  createAccount(registerModel: RegisterModel) {
    let UserDto = JSON.stringify(registerModel);
    const subscription = this.htppClient
      .post('https://localhost:7501/Register', UserDto)
      .pipe(catchError(this.handleError))
      .subscribe({
        next: () => console.log('response'),
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  logIn(loginModel: LoginModel) {
    let UserDto = JSON.stringify(loginModel);
    const subscription = this.htppClient
      .post('https://localhost:7501/Login', UserDto)
      .pipe(catchError(this.handleError))
      .subscribe({
        next: (response) => console.log(response),
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      console.log('Error');
    } else {
      console.log('All good!');
    }
    return throwError(
      () => new Error('Something bad happened; please try again later.')
    );
  }
}
