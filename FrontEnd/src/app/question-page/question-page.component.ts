import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { QuestionButtonComponent } from './question-button/question-button.component';
import { QuestionTextComponent } from './question-text/question-text.component';
import { GameService } from '../services/game.service';
import { catchError, throwError } from 'rxjs';
import { error } from 'console';

@Component({
  selector: 'app-question-page',
  standalone: true,
  imports: [QuestionButtonComponent, QuestionTextComponent],
  templateUrl: './question-page.component.html',
  styleUrl: './question-page.component.css',
})
export class QuestionPageComponent implements OnInit {
  private gameService = inject(GameService);
  private destroyRef = inject(DestroyRef);

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
              // const subscriptionSecondary = this.gameService
              //     .getActiveQuestion()
              //     .pipe(
              //       catchError((error) => {
              //         return throwError(() => new Error(error));
              //     })
              //   )
              //   .subscribe({
              //       next: (response) => {
              //         console.log(response)
              //       },
              //       error: (error) => {
              //         console.log(error)
              //       }
              //     })
              this.gameService.getActiveQuestionx2();
                //this.destroyRef.onDestroy(() => subscriptionSecondary.unsubscribe());
              
            },
            error: (error) => {
              console.log(error);
            },
          });
        
        //this.destroyRef.onDestroy(() => subscription.unsubscribe());
        
  }
}
