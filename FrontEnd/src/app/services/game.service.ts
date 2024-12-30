import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { Question } from "../models/question.model";

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
        return this.httpClient.get<Question>('https://localhost:7500/GetNextQuestion').pipe(
            catchError((error) => {
              const errorMessage = error.error;
              return throwError(() => new Error(errorMessage));
            })
          );
    }

    getActiveQuestionx2(){
        this.httpClient.get('https://localhost:7500/GetNextQuestion').subscribe({next: (data) => console.log(data)})
    }

}