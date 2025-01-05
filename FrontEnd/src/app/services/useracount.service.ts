import { inject, Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { catchError, throwError } from "rxjs";
import { AccountModel } from "../models/account.model";

@Injectable({providedIn : 'root'})
export class UserAccountService{
    private httpClient = inject(HttpClient)

    getUserNameAndEmail(){
    return this.httpClient.get<AccountModel>('https://localhost:7501/GetUserNameAndMail').pipe(
              catchError((error) => {
                const errorMessage = error.error;
                return throwError(() => new Error(errorMessage));
              }
            )
        );

    }
}