import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { LinkButtonComponent } from '../global-components/link-button/link-button.component';
import { StatisticsService } from '../services/statistics.service';
import { catchError, throwError } from 'rxjs';
import { CachedGameSessionList } from '../models/cachedgamesessionlist.model';
import { Router, RouterOutlet } from '@angular/router';


@Component({
  selector: 'app-stats-page',
  standalone: true,
  imports: [LinkButtonComponent, RouterOutlet],
  templateUrl: './stats-page.component.html',
  styleUrl: './stats-page.component.css',
})
export class StatsPageComponent implements OnInit {
  private statisticsService = inject(StatisticsService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
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

  openCachedGameSession(gamesessionId : string)
  {
    this.router.navigate(['stats/details'], {queryParams: {gameSessionId : gamesessionId}});
    
  }
}
