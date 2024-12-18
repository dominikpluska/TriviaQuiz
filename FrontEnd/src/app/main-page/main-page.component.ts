import { Component, DestroyRef, inject } from '@angular/core';
import { ButtonComponent } from '../global-components/button/button.component';
import { LinkButtonComponent } from '../global-components/link-button/link-button.component';
import { Router } from '@angular/router';
import { AuthorizatinService } from '../services/authorizationcalls.service';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [ButtonComponent, LinkButtonComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.css',
})
export class MainPageComponent {
  private router = inject(Router);
  private authenticationService = inject(AuthorizatinService);
  private destroyRef = inject(DestroyRef);

  logOut() {
    const subscription = this.authenticationService
      .logOut()
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          console.log(response);
          this.router.navigate(['/login']);
        },
        error: (error) => {},
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
