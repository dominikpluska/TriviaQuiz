import { inject, Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { catchError, throwError } from "rxjs";
import { LastPlayedGameResult } from "../models/lastplayedgameresults.model";
import { CachedGameSessionList } from "../models/cachedgamesessionlist.model";

@Injectable({providedIn: 'root'})
export class StatisticsService{
  private httpClient = inject(HttpClient)

  getLastPlayedGameSession(){
        return this.httpClient.get<LastPlayedGameResult>('https://localhost:7500/GetLastPlayedGame').pipe(
            catchError((error) => {
              const errorMessage = error.error;
              return throwError(() => new Error(errorMessage));
            })
          );
  }

  getGameSessionList(){
      return this.httpClient.get<[CachedGameSessionList]>('https://localhost:7500/GetAllPlayedGames').pipe(
          catchError((error) => {
            const errorMessage = error.error;
            return throwError(() => new Error(errorMessage));
          })
        );
  }

  getGameSessionStat(gameSessionId : string){
    return this.httpClient.get('https://localhost:7500/GetGameSessionStats?gameSessionId=' + gameSessionId).pipe(
      catchError((error) => {
        const errorMessage = error.error;
        return throwError(() => new Error(errorMessage));
      })
    );
  }


}