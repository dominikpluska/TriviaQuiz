import { Component, DestroyRef, inject, input, OnInit } from '@angular/core';
import { StatisticsService } from '../../services/statistics.service';
import { catchError, throwError } from 'rxjs';
import { CachedGameSession } from '../../models/cachedGameSession.model';

@Component({
  selector: 'app-stats-details-page',
  standalone: true,
  imports: [],
  templateUrl: './stats-details-page.component.html',
  styleUrl: './stats-details-page.component.css'
})
export class StatsDetailsPageComponent implements OnInit {
  gameSessionId = input.required<string>();
  wasAuthorized : boolean = true;
  cachedGameSession? : CachedGameSession;
  errorMessage? : string;
  private statisticsService = inject(StatisticsService);
  private destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    const subscription = this.statisticsService
          .getGameSessionStat(this.gameSessionId())
          .pipe(
            catchError((error) => {
              return throwError(() => new Error(error));
            })
          )
          .subscribe({
            next: (response : CachedGameSession) => {
              this.cachedGameSession = response;
            },
            error: (error) => {
              this.errorMessage = error;
            },
          });
        this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
