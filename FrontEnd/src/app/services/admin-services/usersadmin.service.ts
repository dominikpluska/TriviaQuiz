import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { RegisterAdminModel } from '../../models/admin-models/registeradmin.model';
import { catchError, throwError } from 'rxjs';
import { User } from '../../models/admin-models/user.model';

@Injectable({ providedIn: 'root' })
export class UserAdminService {
  private httpClient = inject(HttpClient);

  getAllUsers() {
    return this.httpClient
      .get<User[]>('https://localhost:7501/admin/GetAllUsers')
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }

  getUser(userId: number) {
    return this.httpClient
      .get<User>('https://localhost:7501/admin/GetUser/' + userId)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }
  registerNewUser(user: RegisterAdminModel) {
    return this.httpClient
      .post('https://localhost:7501/admin/AddNewUser', user)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage.detail));
        })
      );
  }

  updateUser(user: User) {
    return this.httpClient
      .post('https://localhost:7501/admin/UpdateUser', user)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage.detail));
        })
      );
  }

  updatePassword(userId: number, password: string) {
    return this.httpClient
      .post('https://localhost:7501/admin/ChangeUserPassword', {
        userId: userId,
        password: password,
      })
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage.detail));
        })
      );
  }
}
