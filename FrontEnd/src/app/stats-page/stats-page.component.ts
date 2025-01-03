import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { LinkButtonComponent } from '../global-components/link-button/link-button.component';
import { StatisticsService } from '../services/statistics.service';
import { catchError, throwError } from 'rxjs';
import { CachedGameSessionList } from '../models/cachedgamesessionlist.model';

@Component({
  selector: 'app-stats-page',
  standalone: true,
  imports: [LinkButtonComponent],
  templateUrl: './stats-page.component.html',
  styleUrl: './stats-page.component.css',
})
export class StatsPageComponent implements OnInit {
  private statisticsService = inject(StatisticsService);
  private destroyRef = inject(DestroyRef);
  playedGames: (CachedGameSessionList | null)[] = [];

  ngOnInit(){
    const subscription = this.statisticsService
            .getGameSessionList()
            .pipe(
              catchError((error) => {
                return throwError(() => new Error(error));
              })
            )
            .subscribe({
              next: (response : [CachedGameSessionList]) => {
                this.playedGames = response;
              },
              error: (error) => {
              },
            });
          this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
