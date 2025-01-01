import { Component, DestroyRef, inject } from '@angular/core';
import { ButtonComponent } from "../../global-components/button/button.component";
import { GameService } from '../../services/game.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-finish-game-page',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './finish-game-page.component.html',
  styleUrl: './finish-game-page.component.css'
})
export class FinishGamePageComponent {
  private gameService = inject(GameService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);

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
