import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { QuestionLight } from '../../models/admin-models/questionlight.model';
import { QuestionDetails } from '../../models/admin-models/questiondetails.model';

@Injectable({ providedIn: 'root' })
export class QuestionsAdminService {
  private httpClient = inject(HttpClient);

  getAllQuestions() {
    return this.httpClient
      .get<QuestionLight[]>('https://localhost:7500/admin/GetAllQuestions')
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }

  getQuestionDetails(questionId: number) {
    return this.httpClient
      .get<QuestionDetails>(
        'https://localhost:7500/admin/GetQuestionDetails/' + questionId
      )
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage));
        })
      );
  }

  postQuestion(question: QuestionDetails) {
    return this.httpClient
      .post('https://localhost:7500/admin/PostQuestion', question)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          console.log(error);
          return throwError(() => new Error(errorMessage.detail));
        })
      );
  }

  updateQuestion(questionId: number, question: QuestionDetails) {
    return this.httpClient
      .put(
        'https://localhost:7500/admin/UpdateQuestion/' + questionId,
        question
      )
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage.detail));
        })
      );
  }

  deleteQuestion(questionId: number) {
    return this.httpClient
      .delete('https://localhost:7500/admin/DeleteQuestion/' + questionId)
      .pipe(
        catchError((error) => {
          const errorMessage = error.error;
          return throwError(() => new Error(errorMessage.detail));
        })
      );
  }
}
