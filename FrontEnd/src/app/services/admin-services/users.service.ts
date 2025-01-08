import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { User } from '../../models/admin-models/user.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private httpClient: HttpClient = inject(HttpClient);

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
}
