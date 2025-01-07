import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { ButtonComponent } from '../global-components/button/button.component';
import { LinkButtonComponent } from '../global-components/link-button/link-button.component';
import { Router } from '@angular/router';
import { AuthorizatinService } from '../services/authorizationcalls.service';
import { catchError, throwError } from 'rxjs';
import { UserProfileService } from '../services/userprofile.service';
import { GameService } from '../services/game.service';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [ButtonComponent, LinkButtonComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.css',
})
export class MainPageComponent implements OnInit  {
  private router = inject(Router);
  private authenticationService = inject(AuthorizatinService);
  private destroyRef = inject(DestroyRef);
  private userProfileService = inject(UserProfileService);
  private gameService = inject(GameService);
  public userName = this.userProfileService.getUserName;
  public isAdmin = this.userProfileService.getIsAdmin;
  public displayGameOptions : boolean = false;
  public isThereActiveGameSession : boolean = false;
  
  ngOnInit() {
    const subscription = this.gameService
    .checkForActiveGameSession()
    .pipe(
      catchError((error) => {
        return throwError(() => new Error(error));
      })
    )
    .subscribe({
      next: (response : boolean) => {
        if(response === true){
          this.isThereActiveGameSession = true;
        }
        else{
          this.isThereActiveGameSession = false;
        }
        //this.router.navigate(['/game'])
      },
      error: (error) => {
        console.log(error);
      },
    });
  
  this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

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

  requestGameSession(numberOfQuestions? : number){
    const subscription = this.gameService
              .requestGameSession(numberOfQuestions)
              .pipe(
                catchError((error) => {
                  return throwError(() => new Error(error));
                })
              )
              .subscribe({
                next: (response) => {
                  console.log(response)
                  this.router.navigate(['/game'])
                },
                error: (error) => {
                  console.log(error);
                },
              });
            
            this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  showGameOptionsOrRestartTheGame(){
    if(this.isThereActiveGameSession === false){
      this.displayGameOptions = true;
    }
    else{
      this.requestGameSession();
    }
  }
  hideGameOptions(){
    this.displayGameOptions = false;
  }

  
}
