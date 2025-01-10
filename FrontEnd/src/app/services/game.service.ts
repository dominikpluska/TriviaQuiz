import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Question } from '../models/question.model';
import { Answer } from '../models/answer.model';

@Injectable({ providedIn: 'root' })
export class GameService {
  private httpClient: HttpClient = inject(HttpClient);

  requestGameSession(numberOfQuestions?: number) {
    if (numberOfQuestions !== undefined) {
      return this.httpClient
        .get(
          'https://localhost:7500/GetGameSession?numberOfQuestions=' +
            numberOfQuestions
        )
        .pipe(
          catchError((error) => {
            const errorMessage = error.error;
            return throwError(() => new Error(errorMessage));
          })
        );
    } else {
      console.log('test');
      return this.httpClient
        .get('https://localhost:7500/RestartGameSession')
        .pipe(
          catchError((error) => {
            const errorMessage = error.error;
            return throwError(() => new Error(errorMessage));
          })
        );
    }
  }

  getActiveQuestion() {
    return this.httpClient
      .get<Question>('https://localhost:7500/GetNextQuestion')
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }

  checkForActiveGameSession() {
    return this.httpClient
      .get<boolean>('https://localhost:7500/CheckForActiveGameSession')
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }

  postAnswer(answer: Answer) {
    return this.httpClient
      .post('https://localhost:7500/CheckCorrectAnswer', answer)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }

  closeGameSession() {
    return this.httpClient
      .post('https://localhost:7500/CloseGameSession', null)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }
}
