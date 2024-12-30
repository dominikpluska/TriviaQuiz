import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, throwError } from "rxjs";

@Injectable({ providedIn: 'root' })
export class GameService{
    private httpClient: HttpClient = inject(HttpClient);

    requestGameSession(){
         return this.httpClient.get('https://localhost:7500/GetGameSession').pipe(
              catchError((error) => {
                const errorMessage = error.error;
                return throwError(() => new Error(errorMessage));
              })
            );
    }

    getActiveQuestion(){
        return this.httpClient.get('https://localhost:7500/GetRandomQuestion').pipe(
            catchError((error) => {
              const errorMessage = error.error;
              return throwError(() => new Error(errorMessage));
            })
          );
    }

}