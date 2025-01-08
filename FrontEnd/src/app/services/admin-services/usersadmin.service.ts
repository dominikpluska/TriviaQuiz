import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { RegisterAdminModel } from '../../models/admin-models/registeradmin.model';
import { catchError, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserAdminService {
  private httpClient = inject(HttpClient);

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
}
