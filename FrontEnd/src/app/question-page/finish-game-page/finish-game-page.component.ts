import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ButtonComponent } from "../../global-components/button/button.component";
import { GameService } from '../../services/game.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { StatisticsService } from '../../services/statistics.service';
import { LastPlayedGameResult } from '../../models/lastplayedgameresults.model';

@Component({
  selector: 'app-finish-game-page',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './finish-game-page.component.html',
  styleUrl: './finish-game-page.component.css'
})
export class FinishGamePageComponent implements OnInit {
  private gameService = inject(GameService);
  private statisticsService = inject(StatisticsService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
  gameResults?: LastPlayedGameResult;

  ngOnInit() {
    const subscription = this.statisticsService
        .getLastPlayedGameSession()
        .pipe(
          catchError((error) => {
            return throwError(() => new Error(error));
          })
        )
        .subscribe({
          next: (response : LastPlayedGameResult) => {
            console.log(response);
            this.gameResults = response;
          },
          error: (error) => {
          },
        });
      this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  closeGameSession(){
    const subscription = this.gameService
        .closeGameSession()
        .pipe(
          catchError((error) => {
            return throwError(() => new Error(error));
          })
        )
        .subscribe({
          next: (response) => {
            console.log(response)
            this.router.navigate(['/main'])
          },
          error: (error) => {
          },
        });
      this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
