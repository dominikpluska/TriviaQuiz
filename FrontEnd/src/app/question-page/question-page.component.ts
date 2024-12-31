import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { QuestionButtonComponent } from './question-button/question-button.component';
import { QuestionTextComponent } from './question-text/question-text.component';
import { GameService } from '../services/game.service';
import { catchError, throwError } from 'rxjs';
import { Question } from '../models/question.model';
import { Answer } from '../models/answer.model';
import { ButtonComponent } from "../global-components/button/button.component";

@Component({
  selector: 'app-question-page',
  standalone: true,
  imports: [QuestionButtonComponent, QuestionTextComponent, ButtonComponent],
  templateUrl: './question-page.component.html',
  styleUrl: './question-page.component.css',
})
export class QuestionPageComponent implements OnInit {
  private gameService = inject(GameService);
  private destroyRef = inject(DestroyRef);
  private selectedAnswer : Answer = {Answer : "", QuestionId : 0};
  currentQuestion = signal<any>(null);
  wasCorrect : string = 'empty';
 
  ngOnInit() {
    const subscription = this.gameService
          .requestGameSession()
          .pipe(
            catchError((error) => {
              return throwError(() => new Error(error));
            })
          )
          .subscribe({
            next: (response) => {
              const subscriptionSecondary = this.gameService
                  .getActiveQuestion()
                  .pipe(
                    catchError((error) => {
                      return throwError(() => new Error(error));
                  })
                )
                .subscribe({
                    next: (response : Question) => {
                      this.currentQuestion.set(response);
                    },
                    error: (error) => {
                      console.log(error)
                    }
                  })
                this.destroyRef.onDestroy(() => subscriptionSecondary.unsubscribe());
              
            },
            error: (error) => {
              console.log(error);
            },
          });
        
        this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  handleAnswer(answer : string){
    this.selectedAnswer.QuestionId = this.currentQuestion().questionId;
    this.selectedAnswer.Answer = answer;
    console.log(this.selectedAnswer);

    const subscription = this.gameService
    .postAnswer(this.selectedAnswer)
    .pipe(
      catchError((error) => {
        return throwError(() => new Error(error));
      })
    )
    .subscribe({
      next: (response) => {
        console.log(response.toString())
        this.wasCorrect = response.toString();
      },
      error: (error) => {
      },
    });
  this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  requestNextQuestion(){
    this.wasCorrect = 'empty';

    const subscriptionSecondary = this.gameService
      .getActiveQuestion()
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
      })
    )
    .subscribe({
        next: (response : Question) => {
          this.currentQuestion.set(response);
        },
        error: (error) => {
          console.log(error)
        }
      })
    this.destroyRef.onDestroy(() => subscriptionSecondary.unsubscribe());
  }
}
