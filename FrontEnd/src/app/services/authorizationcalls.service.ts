import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { RegisterModel } from '../models/register.model';
import { LoginModel } from '../models/login.model';
import { catchError, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthorizatinService {
  private htppClient: HttpClient = inject(HttpClient);

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

  checkAuthorization() {
    return this.htppClient.get('https://localhost:7501/AuthCheck').pipe(
      catchError((error) => {
        const errorMessage = error.error;
        return throwError(() => new Error(errorMessage));
      })
    );
  }

  logOut() {
    return this.htppClient.get('https://localhost:7501/LogOut').pipe(
      catchError((error) => {
        const errorMessage = error.error;
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
