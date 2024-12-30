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

  ngOnInit(): void {
    const subscription = this.gameService
          .requestGameSession()
          .pipe(
            catchError((error) => {
              return throwError(() => new Error(error));
            })
          )
          .subscribe({
            next: (response) => {
              console.log("Success! Get another question now! :)")
              
            },
            error: (error) => {
              console.log(error);
            },
          });
        
        this.destroyRef.onDestroy(() => subscription.unsubscribe());
        const subscriptionSecondary = this.gameService
                  .getActiveQuestion()
                  .pipe(
                    catchError((error) => {
                      return throwError(() => new Error(error));
                  })
                )
                .subscribe({
                    next: (response : any) => {
                      console.log(response)
                    },
                    error: (error) => {
                      console.log(error)
                    }
                  })
         this.destroyRef.onDestroy(() => subscriptionSecondary.unsubscribe());
  }
}
