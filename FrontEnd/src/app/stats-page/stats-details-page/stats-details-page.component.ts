import {
  ChangeDetectorRef,
  Component,
  DestroyRef,
  inject,
  input,
  NgZone,
  OnInit,
  signal,
} from '@angular/core';
import { StatisticsService } from '../../services/statistics.service';
import { catchError, throwError } from 'rxjs';
import { CachedGameSession } from '../../models/cachedGameSession.model';
import { LinkButtonComponent } from '../../global-components/link-button/link-button.component';
import { BoolTransformerPipe } from '../../custom-pipes/booltransformer.pipe';

@Component({
  selector: 'app-stats-details-page',
  standalone: true,
  imports: [LinkButtonComponent, BoolTransformerPipe],
  templateUrl: './stats-details-page.component.html',
  styleUrl: './stats-details-page.component.css',
})
export class StatsDetailsPageComponent implements OnInit {
  gameSessionId = input.required<string>();
  wasAuthorized: boolean = true;
  cachedGameSession?: CachedGameSession | null;
  errorMessage?: string;
  private statisticsService = inject(StatisticsService);
  private destroyRef = inject(DestroyRef);

  ngOnInit() {
    if (this.gameSessionId() === '') {
      this.errorMessage = 'GameSessionId is empty!';
    } else {
      const subscription = this.statisticsService
        .getGameSessionStat(this.gameSessionId())
        .pipe(
          catchError((error) => {
            return throwError(() => new Error(error));
          })
        )
        .subscribe({
          next: (response: CachedGameSession) => {
            this.cachedGameSession = response;
            console.log(response);
          },
          error: (error) => {
            this.errorMessage = error;
          },
        });
      this.destroyRef.onDestroy(() => subscription.unsubscribe());
    }
  }
}
